using BlockCovid.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Interfaces
{
    public interface ICitizensRepository
    {
       // IQueryable<Citizen> Citizens { get; }

        Task<List<Citizen>> GetCitizensAsync();
        Task<Citizen> GetCitizenByIdAsync(long id);
        Task<Citizen> CreateCitizensAsync(Citizen citizen);
        
    }
}
