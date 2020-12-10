using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlockCovid.Models;
using BlockCovid.Models.Dto;

namespace BlockCovid.Utils
{
    public class ServiceMap : Profile
    {
        public ServiceMap()
        {
            CreateMap<Participant, ParticipantDto>();
            CreateMap<Participant, ParticipantConnexionDto>();

            CreateMap<Citizen, CitizenDto>();
            CreateMap<CitizenDto, Citizen>();

            CreateMap<Citizen, CitizenRegisterDto>();
            CreateMap<CitizenRegisterDto, Citizen>();

           
            CreateMap<QrCode, QrCodeDto>();
            CreateMap<QrCodeDto, QrCode>();
        }
    }
}
