
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlockCovid.Dal;
using BlockCovid.Models;
using BlockCovid.Interfaces;
using BlockCovid.Models.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using System.IdentityModel.Tokens.Jwt;
using BlockCovid.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Security.Claims;

namespace BlockCovid.Controllers
{
    [EnableCors("_myAllowSpecificOrigins")]
    [Produces("application/json")] // ce que le controler va renvoyer
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
    
        private readonly IParticipantsRepository _participant;
        private readonly BlockCovidContext _blockCovid;
        private readonly IMapper _mapper;


        public ParticipantsController(IParticipantsRepository participant, BlockCovidContext blockCovid, IMapper mapper)
        {
            _participant = participant;
            _blockCovid = blockCovid;
            _mapper = mapper;
        }

        // GET: api/Participants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> GetParticipants()
        {
            return await _blockCovid.Participants.Select(x => _mapper.Map<ParticipantDto>(x)).ToListAsync();
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

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        public async Task<ActionResult<ParticipantDto>> PostParticipant(ParticipantDto participantDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //TODO: check si l'email existe ou pas
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(participantDTO.Password);

            //Participant participant = _mapper.Map<Participant>(participantDTO);
            
            var participant = new Participant
            {
                Login = participantDTO.Login,
                Password = passwordHash,
                Participant_Type = (ParticipantType) participantDTO.Participant_Type
            };
            
            _blockCovid.Participants.Add(participant);
            await _blockCovid.SaveChangesAsync();

            var tokenJWT = Token.createToken(participant);

            //return CreatedAtAction("GetParticipant", new { id = participant.ParticipantID }, _mapper.Map<ParticipantDto>(participant));
            return Ok(new JwtSecurityTokenHandler().WriteToken(tokenJWT));

        }

        // POST: api/Participants/login
        [HttpPost("login")]
        public async Task<ActionResult<ParticipantConnexionDto>> Login(ParticipantConnexionDto participantConnexionDto)
        {
            Participant participant = 
                await _blockCovid.Participants
                .Where(participant => participant.Login == participantConnexionDto.Login)
                .FirstOrDefaultAsync();

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

            
            var tokenJWT = Token.createToken(participant);

            return Ok(new JwtSecurityTokenHandler().WriteToken(tokenJWT));
        }
    }
}
