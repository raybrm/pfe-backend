using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockCovid.Models;
namespace BlockCovid.Interfaces
{
    public interface IParticipantsRepository
    {
        Task<List<Participant>> GetParticipantsAsync();
        Task<Participant> GetParticipantByIdAsync(long id);
        Task<Participant> CreateParticipantsAsync(Participant participant);
    }
}
