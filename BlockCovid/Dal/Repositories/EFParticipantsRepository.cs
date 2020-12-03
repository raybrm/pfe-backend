using BlockCovid.Interfaces;
using BlockCovid.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Dal.Repositories
{
    public class EFParticipantsRepository : IParticipantsRepository
    {
        private readonly BlockCovidContext _context;
        public EFParticipantsRepository(BlockCovidContext context)
        {
            _context = context;
        }
        public async Task<Participant> CreateParticipantsAsync(Participant participant)
        {
           
            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();
            return participant;
        }

        public async Task<Participant> GetParticipantByIdAsync(long id)
        {
            return await _context.Participants.FindAsync(id);
        }

        public async Task<List<Participant>> GetParticipantsAsync()
        {
            return await _context.Participants.ToListAsync();
        }

        private bool ParticipantExists(long id)
        {
            return _context.Participants.Any(e => e.ParticipantID == id);
        }
    }
}
