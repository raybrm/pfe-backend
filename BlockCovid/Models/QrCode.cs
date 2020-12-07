using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlockCovid.Models
{
    public class QrCode
    {
        public QrCode()
        {
            Citizen_Qr_Code = new HashSet<CitizenQrCode>();
        }

        public String QrCodeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // Foreign Key
        public long ParticipantID { get; set; }
        // Navigation property
        public virtual Participant Participant { get; set; }
        public virtual ICollection<CitizenQrCode> Citizen_Qr_Code { get; set; }
    }
}
