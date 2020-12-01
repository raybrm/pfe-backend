using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models
{
    public class QrCode
    {
        public QrCode()
        {
            Citizen_Qr_Code = new HashSet<CitizenQrCode>();
        }

        public long QrCodeID { get; set; }
        public string Name { get; set; }
        public string Descritpion { get; set; }
        [Required]
        public virtual Participant Participant { get; set; }
        public virtual ICollection<CitizenQrCode> Citizen_Qr_Code { get; set; }
    }
}
