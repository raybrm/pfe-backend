﻿
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlockCovid.Models;
using BlockCovid.Interfaces;
using BlockCovid.Models.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using System.IdentityModel.Tokens.Jwt;
using BlockCovid.Utils;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace BlockCovid.Controllers
{
    [EnableCors("_myAllowSpecificOrigins")]
    [Produces("application/json")] // ce que le controler va renvoyer
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
    
        private readonly IParticipantsRepository _participant;     
        private readonly JWTSettings _jwtSettings;


        public ParticipantsController(IParticipantsRepository participant,  JWTSettings jwtSettings)
        {
            _participant = participant;
            _jwtSettings = jwtSettings;
        }

        /// <summary>
        /// Permet de recuperer tous les participants
        /// </summary>
        /// <response code="200">Recup la liste de tous les participants </response>
        // GET: api/Participants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> GetParticipants()
        {
            return await _participant.GetParticipantsAsync();
        }


        /// <summary>
        /// Permet d'enregistrer un participant
        /// </summary>
        /// <response code="200">Participant enregistré</response>
        /// <response code="400">Si l'utilisateur ne respecte pas model ParticipantDto</response>  
        /// <response code="409">Si login est déjà utilisé</response>  
        /// <response code="412">Si mdp est différent du champ confirmé mdp</response>  
        // POST: api/Participants
        [HttpPost("register")]
        public async Task<ActionResult<ParticipantDto>> PostParticipant(ParticipantDto participantDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            } 

            if (!participantDTO.ConfirmPassword.Equals(participantDTO.Password))
            {
                return StatusCode(412);
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(participantDTO.Password);
            
            var participant = new Participant
            {
                Login = participantDTO.Login,
                Password = passwordHash,
                Participant_Type = (ParticipantType) participantDTO.Participant_Type
            };

            try
            {
               await _participant.CreateParticipantsAsync(participant);
            }
            catch (DbUpdateException)
            {
                return Conflict(new { message = "The login already exist" });
            }

            var tokenJWT = GenerateJWToken(participant);

            return Ok(new JwtSecurityTokenHandler().WriteToken(tokenJWT));

        }

        /// <summary>
        /// Permet de se connecter
        /// </summary>
        /// <response code="200">Connecté</response>
        /// <response code="400">Si l'utilisateur ne respecte pas model ParticipantConnexionDto, ou bien que mail ou mdp est mauvais</response>  
        // POST: api/Participants/login
        [HttpPost("login")]
        public async Task<ActionResult<ParticipantConnexionDto>> Login(ParticipantConnexionDto participantConnexionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Participant participant = await _participant.GetParticipantByLoginAsync(participantConnexionDto.Login);

            if (participant == null)
            {
                return BadRequest(new { message = "Wrong password or wrong mail"});
            }

            string passwordHash = participant.Password;
            bool verified = BCrypt.Net.BCrypt.Verify(participantConnexionDto.Password, passwordHash);

            if (!verified)
            {
                return BadRequest(new { message = "Wrong password or wrong mail" });
            }

            
            var tokenJWT = GenerateJWToken(participant);

            return Ok(new JwtSecurityTokenHandler().WriteToken(tokenJWT));
        }

        /// <summary>
        /// Permet de renvoyer le role du participant
        /// </summary>
        /// <response code="200">Role renvoyé</response>
        //api/Participants/verification
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("verification")]
        public ActionResult Verification()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string role = identity.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;

            System.Diagnostics.Debug.WriteLine(role);

            return Ok(new { role = role });

        }

        private JwtSecurityToken GenerateJWToken(Participant participant)
        {
            var SIGNING_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret_key));
            var signingCredentials = new SigningCredentials(SIGNING_KEY, SecurityAlgorithms.HmacSha256);

            var role = participant.Participant_Type.ToString();
            var claims = new List<Claim>();
            claims.Add(new Claim("login", participant.Login));
            claims.Add(new Claim(ClaimTypes.Role, role));

            var jwtSecurityToken = new JwtSecurityToken(
               issuer: _jwtSettings.Issuer,
               audience: _jwtSettings.Audience,
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
               signingCredentials: signingCredentials
               );

            return jwtSecurityToken;
        }
    }
}
