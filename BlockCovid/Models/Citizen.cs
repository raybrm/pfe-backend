

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlockCovid.Models
{
    public class Citizen
    {
        public Citizen()
        {
            Citizen_QrCode = new HashSet<CitizenQrCode>();
        }

        public long CitizenID { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        [Required]
        public bool Is_Positive { get; set; }

        public virtual ICollection<CitizenQrCode> Citizen_QrCode { get; set; }

    }
}
