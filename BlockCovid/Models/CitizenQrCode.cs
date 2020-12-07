using System;
using System.Collections.Generic;

#nullable disable

namespace BlockCovid.Models
{
    public partial class CitizenQrCode
    {

        public long CitizenQrCodeId { get; set; }
        public DateTime Timestamp { get; set; }
        public String QrCodeId { get; set; }
        public long CitizenId { get; set; }
        //Navigation property
        public virtual Citizen Citizen { get; set; }
        public virtual QrCode QrCode { get; set; }
    }
}
