using BlockCovid.Models.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models
{
    public class ComparateurCitizens : IEqualityComparer<Citizen>
    {
        public bool Equals(Citizen x, Citizen y)
        {
            return x.CitizenID==y.CitizenID;
        }

        public int GetHashCode([DisallowNull] Citizen obj)
        {
            return obj.CitizenID.GetHashCode();
        }
    }
}
