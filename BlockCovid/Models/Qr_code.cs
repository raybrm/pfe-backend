using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models
{
    public class Qr_code
    {
        public long QrCode_ID { get; set; }
        public long Participant_Participant_ID { get; set; }
        public string Name { get; set; }
        public string Descritpion { get; set; }
        public virtual Participant Participant { get; set; }
        public virtual List<Citizen_qr_code> Citizen_Qr_Code { get; set; }
    }
}
