using System;
using System.ComponentModel.DataAnnotations;

namespace BlockCovid.Models
{
    public class Citizen_qr_code
    {
        public long Citizen_QrCode_ID { get; set; }
        public long Citizen_Citizen_ID { get; set; }
        public long QrCode_QrCode_ID { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual Qr_code Qrcode { get; set; }
        public virtual Citizen Citizen { get; set; }
    }
}
