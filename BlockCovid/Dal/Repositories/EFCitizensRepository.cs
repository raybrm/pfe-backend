using BlockCovid.Interfaces;
using BlockCovid.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Dal.Repositories
{
    public class EFCitizensRepository : ICitizensRepository
    {
        private readonly BlockCovidContext _context;

        public EFCitizensRepository(BlockCovidContext context)
        {
            _context = context;
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



        public async Task<Citizen> ToNotify(long id)
        {
            IQueryable<CitizenQrCode> listCustomer = from CitizenQrCode c in _context.Citizens
                                                        where c.CitizenId==id
                                                        select c ;

            foreach(CitizenQrCode citizenQrCode in listCustomer)
            {

                System.Diagnostics.Debug.WriteLine(citizenQrCode.Timestamp);
               
            }

            Citizen citizenQrCod = null;
            return await Task.FromResult(citizenQrCod);
        }

    }
}
