using System.Collections.Generic;
using System.Threading.Tasks;
using BlockCovid.Models;
using BlockCovid.Models.Dto;

namespace BlockCovid.Interfaces
{
    public interface IParticipantsRepository
    {


        Task<List<ParticipantDto>> GetParticipantsAsync();
        Task<Participant> GetParticipantByIdAsync(long id);
        Task<Participant> CreateParticipantsAsync(Participant participant);
        Task<Participant> GetParticipantByLoginAsync(string login);
    }
}
