﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlockCovid.Models;
using BlockCovid.Models.Dto;

namespace BlockCovid.Services
{
    public class ServiceMap : Profile
    {
        public ServiceMap()
        {
            CreateMap<Participant, ParticipantDto>();
<<<<<<< HEAD
            CreateMap<CitizenQrCode, CitizenQrCodeDto>();
           // CreateMap<ParticipantDto, Participant>();
=======

            CreateMap<Participant, ParticipantConnexionDto>();
>>>>>>> 335d7b644088deadbab27dc3080f988cd0a0f8a2
        }
    }
}
