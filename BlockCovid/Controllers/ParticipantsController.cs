﻿using System;
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
    [Produces("application/json")] // ce que le controler va renvoyer
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
    
        private readonly IParticipantsRepository _participant;

        public ParticipantsController(IParticipantsRepository participant)
        {
            _participant = participant;
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
            var participant = await _participant.GetParticipantByIdAsync(id);

            if (participant == null)
            {
                return NotFound();
            }

            return participant;
        }

        // POST: api/Participants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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
                Participant_Type = (ParticipantType)participantDTO.Participant_Type
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
