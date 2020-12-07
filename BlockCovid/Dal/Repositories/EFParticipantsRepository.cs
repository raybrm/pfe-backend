using AutoMapper;
using BlockCovid.Interfaces;
using BlockCovid.Models;
using BlockCovid.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Dal.Repositories
{
    public class EFParticipantsRepository : IParticipantsRepository
    {
        private readonly BlockCovidContext _context;
        private readonly IMapper _mapper;
        public EFParticipantsRepository(BlockCovidContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        public async Task<List<ParticipantDto>> GetParticipantsAsync()
        {
            return await _context.Participants.Select(x => _mapper.Map<ParticipantDto>(x)).ToListAsync();
        }

        public async Task<Participant> GetParticipantByLoginAsync(string login)
        {
            return await _context.Participants
                .Where(p => p.Login == login)
                .FirstOrDefaultAsync();
        }
    }
}
