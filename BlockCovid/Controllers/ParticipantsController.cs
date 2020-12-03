using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlockCovid.Dal;
using BlockCovid.Models;
using BlockCovid.Dal.Repositories;
using BlockCovid.Interfaces;
using BlockCovid.Models.Dto;
using BlockCovid.Services;
using AutoMapper;

namespace BlockCovid.Controllers
{
    [Produces("application/json")] // ce que le controler va renvoyer
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
    
        private readonly IParticipantsRepository _participant;
        private readonly BlockCovidContext _blockCovid;
        private readonly IMapper _mapper;
        public ParticipantsController(IParticipantsRepository participant, BlockCovidContext blockCovid,  IMapper mapper)
        {
            _mapper = mapper;
            _participant = participant;
            _blockCovid = blockCovid;
        }

        // GET: api/Participants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> GetParticipants()
        {
            
            
            return await  _blockCovid.Participants.Select(x => _mapper.Map<ParticipantDto>(x)).ToListAsync();
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
            //System.Diagnostics.Debug.WriteLine("OKOPKPOKPOOPKPOKOPKOPKPOKOKOPKOPOKKLBKHKJHKKJHHKJHKHKJ");
            //System.Diagnostics.Debug.WriteLine(participantDto.Login + " " + participantDto.Password + " " + participantDto.Participant_Type + " " + participant.Password);
            return participantDto;
        }

        // POST: api/Participants

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[Route("/register")]
        [HttpPost]
        public async Task<ActionResult<ParticipantDto>> PostParticipant(ParticipantDto participantDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(participantDTO.Password);


            var participant = new Participant
            {
                Login = participantDTO.Login,
                Password = passwordHash,
                Participant_Type = participantDTO.Participant_Type
            };

            _blockCovid.Participants.Add(participant);
            await _blockCovid.SaveChangesAsync();
            //return Ok();
            return CreatedAtAction("GetParticipant", new { id = participant.ParticipantID }, ParticipantToDTO(participant));
 
        }

        private bool ParticipantExists(long id)
        {
            return _blockCovid.Participants.Any(e => e.ParticipantID == id);
        }

        private static ParticipantDto ParticipantToDTO(Participant participant)
        {
            return new ParticipantDto
            {
                Login = participant.Login,
                Password = participant.Password,
                Participant_Type = participant.Participant_Type
            };
        }
    }
}
