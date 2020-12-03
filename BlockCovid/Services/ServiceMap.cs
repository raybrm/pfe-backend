using System;
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
            CreateMap<CitizenQrCode, CitizenQrCodeDto>();
           // CreateMap<ParticipantDto, Participant>();
        }
    }
}
