using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models.Dto
{
    public class QrCodeDto
    {
        public String QrCodeID { get; set; }
        public string Name { get; set; }
        public string Descritpion { get; set; }
        // Foreign Key
        public long ParticipantID { get; set; }
    }
}
