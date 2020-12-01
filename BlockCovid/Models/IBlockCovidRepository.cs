using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models
{
    interface IBlockCovidRepository
    {
        IQueryable<Citizen> Citizens { get; }
        IQueryable<Participant> Participants { get; }
    }
}
