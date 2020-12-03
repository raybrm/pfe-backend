using BlockCovid.Interfaces;
using BlockCovid.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlockCovid.Models.Dto;

namespace BlockCovid.Dal.Repositories
{
    public class EFCitizensRepository : ICitizensRepository
    {
        private readonly BlockCovidContext _context;
        private readonly IMapper _mapper;
        public EFCitizensRepository(BlockCovidContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<Citizen>> GetCitizensAsync()
        {
            return await _context.Citizens.ToListAsync();
        }
        

        public async Task<Citizen> GetCitizenByIdAsync(long id)
        {
            return await _context.Citizens.FindAsync(id);
        }

        public async Task<Citizen> CreateCitizensAsync(Citizen citizen)
        {
            System.Diagnostics.Debug.WriteLine("OKOPKPOKPOOPKPOKOPKOPKPOKOKOPKOPOKKLBKHKJHKKJHHKJHKHKJ");
            System.Diagnostics.Debug.WriteLine(citizen);
            _context.Citizens.Add(citizen);
            await _context.SaveChangesAsync();
            return citizen;
        }


        public bool CitizenExists(long id)
        {
            return _context.Citizens.Any(e => e.CitizenID == id);
        }



        public void ToNotify(long id)
        {
            
           
            IQueryable<CitizenQrCodeDto> listCustomer = from CitizenQrCode c in _context.CitizenQrCode
                                                        where c.CitizenId==1
                                                        select _mapper.Map<CitizenQrCodeDto>(c) ;
            
            foreach(CitizenQrCodeDto citizenQrCode in listCustomer)
            {

                System.Diagnostics.Debug.WriteLine(citizenQrCode.Timestamp+" "+citizenQrCode.CitizenQrCodeId);
                
                DateTime datePlusUneHeure = citizenQrCode.Timestamp.AddHours(1);
                DateTime dateMoinsUneHeure = citizenQrCode.Timestamp.AddHours(-1);
               
            }

            
        }

    }
}
