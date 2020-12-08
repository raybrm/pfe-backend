﻿using BlockCovid.Models;
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
        Task<Citizen> GetCitizenByIdAsync(long id);
        Task<Citizen> CreateCitizensAsync(Citizen citizen);
        Task<CitizenDto> IfCitizenInDbAsync(CitizenDto citizenDto);
        Task<CitizenDto> UpdateCitizen(CitizenDto citizenDto);
        Task<CitizenDto> UpdateCitizenToken(CitizenDto citizenDto);

    }
}
