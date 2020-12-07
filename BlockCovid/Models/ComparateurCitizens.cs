using BlockCovid.Models.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models
{
    public class ComparateurCitizens : IEqualityComparer<CitizenDto>
    {
        public bool Equals(CitizenDto x, CitizenDto y)
        {
            return x.CitizenID==y.CitizenID;
        }

        public int GetHashCode([DisallowNull] CitizenDto obj)
        {
            return obj.CitizenID.GetHashCode();
        }
    }
}
