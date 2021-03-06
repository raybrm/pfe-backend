﻿using BlockCovid.Interfaces;
using BlockCovid.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlockCovid.Models.Dto;
using System.Text;
using System.Net;
using System.IO;
using System.Text.Json;
using BlockCovid.ConfigurationSettings;
using Microsoft.Extensions.Options;

namespace BlockCovid.Dal.Repositories
{
    public class EFCitizensRepository : ICitizensRepository
    {
        private readonly BlockCovidContext _context;
        private readonly IMapper _mapper;
        private IOptions<FireBaseSettings> _fireBaseSettings;
        public EFCitizensRepository(BlockCovidContext context, IMapper mapper, IOptions<FireBaseSettings> fireBaseSettings)
        {
            _context = context;
            _fireBaseSettings = fireBaseSettings;
            _mapper = mapper;
        }

        public async Task<CitizenDto> GetCitizenByIdAsync(long id)
        {
            var citizen = await _context.Citizens.FindAsync(id);

            return _mapper.Map<CitizenDto>(citizen);
        }
        

        public async Task<CitizenDto> CreateCitizensAsync(CitizenRegisterDto citizenDto)
        {
           
            var citizen = _context.Citizens.Add(_mapper.Map<Citizen>(citizenDto));
            await _context.SaveChangesAsync();
            return _mapper.Map<CitizenDto>(citizen.Entity);
        }


        public bool CitizenExists(long id)
        {
            return _context.Citizens.Any(e => e.CitizenID == id);
        }

        public Task<CitizenDto> IfCitizenInDbAsync(CitizenDto citizenDto)
        {
            CitizenDto cDto = (from Citizen c in _context.Citizens
                                                        where c.CitizenID == citizenDto.CitizenID
                                     select _mapper.Map<CitizenDto>(c)).FirstOrDefault();
           
            return Task.FromResult(cDto);
        }
        public async Task<CitizenDto> UpdateCitizenToken(CitizenDto citizenDto)
        {
            Citizen citizen = (from Citizen c in _context.Citizens
                               where c.CitizenID == citizenDto.CitizenID
                               select c).SingleOrDefault();
            citizen.TokenFireBase = citizenDto.TokenFireBase;
            await _context.SaveChangesAsync();
            return citizenDto;
        }
        public async  Task<CitizenDto> UpdateCitizen(CitizenDto citizenDto)
        {
            Citizen citizen = (from Citizen c in _context.Citizens
                               where c.CitizenID == citizenDto.CitizenID
                               select c).SingleOrDefault();
            citizen.First_Name = citizenDto.First_Name;
            citizen.Last_Name = citizenDto.Last_Name;
            citizen.TokenFireBase = citizenDto.TokenFireBase;
            await _context.SaveChangesAsync();
            return citizenDto;
        }
    }
}
