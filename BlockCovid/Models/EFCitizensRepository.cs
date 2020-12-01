using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models
{
    public class EFCitizensRepository : ICitizensRepository
    {
        private BlockCovidContext context;

        public EFCitizensRepository(BlockCovidContext context)
        {
            this.context = context;
        }

        public IQueryable<Citizen> Citizens => context.Citizens;

    }
}
