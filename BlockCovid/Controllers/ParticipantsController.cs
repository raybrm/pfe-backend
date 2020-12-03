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

namespace BlockCovid.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly BlockCovidContext _context;
        //private readonly IParticipantsRepository _participant;

        public ParticipantsController(BlockCovidContext context)
        {
            _context = context;
        }

        // GET: api/Participants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParticipantDto>>> GetParticipants()
        {
            
            return await _context.Participants.Select(x=>ParticipantToDTO(x)).ToListAsync();
        }

        // GET: api/Participants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Participant>> GetParticipant(long id)
        {
            var participant = await _context.Participants.FindAsync(id);

            if (participant == null)
            {
                return NotFound();
            }

            return participant;
        }

        // PUT: api/Participants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParticipant(long id, Participant participant)
        {
            if (id != participant.ParticipantID)
            {
                return BadRequest();
            }

            _context.Entry(participant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticipantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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
            var participant = new Participant
            {
                Login = participantDTO.Login,
                Password = participantDTO.Password,
                Participant_Type = participantDTO.Participant_Type
            };

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();
            //return Ok();
            return CreatedAtAction("GetParticipant", new { id = participant.ParticipantID }, ParticipantToDTO(participant));
 
        }

        private bool ParticipantExists(long id)
        {
            return _context.Participants.Any(e => e.ParticipantID == id);
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
