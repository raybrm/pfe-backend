using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models.Dto
{
    public class CitizenQrCodeDto
    {
        public long CitizenQrCodeId { get; set; }
        public DateTime Timestamp { get; set; }
        public String QrCodeId { get; set; }
        public long CitizenId { get; set; }
    }
}
