using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models
{
    public interface ICitizensRepository
    {
        IQueryable<Citizen> Citizens { get; }
    }
}
