﻿using System;
using System.Collections.Generic;

#nullable disable

namespace BlockCovid.Models
{
    public partial class CitizenQrCode
    {

        public long CitizenQrCodeId { get; set; }
        public DateTime Timestamp { get; set; }
        public long QrCodeId { get; set; }
        public long CitizenId { get; set; }

        public virtual Citizen Citizen { get; set; }
        public virtual QrCode QrCode { get; set; }
    }
}