using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Services
{
    public class JWTSettings
    {
        public string Secret_key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInMinutes { get; set; }

    }
}
