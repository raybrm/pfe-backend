using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models.Dto
{
    public class CitizenDto
    {
        //TODO: Mettre required là où nous en avons besoin
        
        public long CitizenID { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        [Required]
        public string TokenFireBase { get; set; }
        [Required]
        public bool Is_Positive { get; set; }
    }
}
