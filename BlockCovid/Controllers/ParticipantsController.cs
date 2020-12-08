
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
using BlockCovid.Services;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BlockCovid.Controllers
{
    [EnableCors("_myAllowSpecificOrigins")]
    [Produces("application/json")] // ce que le controler va renvoyer
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
    
        private readonly IParticipantsRepository _participant;
        private readonly IMapper _mapper;
        private readonly JWTSettings _jwtSettings;


        public ParticipantsController(IParticipantsRepository participant, IMapper mapper, JWTSettings jwtSettings)
        {
            _participant = participant;
            _mapper = mapper;
            _jwtSettings = jwtSettings;
        }

        // GET: api/Participants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> GetParticipants()
        {
            return await _participant.GetParticipantsAsync();
        }

        // GET: api/Participants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParticipantDto>> GetParticipant(long id)
        {
            var participant = await _participant.GetParticipantByIdAsync(id);

            if (participant == null)
            {
                return NotFound();
            }

            var participantDto = _mapper.Map<ParticipantDto>(participant);
            return participantDto;
        }

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

            //TODO: Participant participant = _mapper.Map<Participant>(participantDTO);
            
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

            //return CreatedAtAction("GetParticipant", new { id = participant.ParticipantID }, _mapper.Map<ParticipantDto>(participant));
            return Ok(new JwtSecurityTokenHandler().WriteToken(tokenJWT));

        }

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
