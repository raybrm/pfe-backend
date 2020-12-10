using BlockCovid.Models;
using BlockCovid.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Interfaces
{
    public interface ICitizensRepository
    {
        Task<CitizenDto> GetCitizenByIdAsync(long id);
        Task<CitizenDto> CreateCitizensAsync(CitizenRegisterDto citizen);
        Task<CitizenDto> IfCitizenInDbAsync(CitizenDto citizenDto);
        Task<CitizenDto> UpdateCitizen(CitizenDto citizenDto);
        Task<CitizenDto> UpdateCitizenToken(CitizenDto citizenDto);

    }
}
