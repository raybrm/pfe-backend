using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models.Dto
{
    public class CitizenRegisterDto
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string TokenFireBase { get; set; }
        public bool Is_Positive { get; set; }
        public bool Is_Exposed { get; set; }
    }
}
