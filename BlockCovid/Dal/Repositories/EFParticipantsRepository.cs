using BlockCovid.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Dal.Repositories
{
    public class EFParticipantsRepository : IParticipantsRepository
    {
        private readonly BlockCovidContext _context;

    }
}
