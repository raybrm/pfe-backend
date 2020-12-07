using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models.Dto
{
    public class CitizenQrCodeDto
    {
        [Required]
        public long CitizenQrCodeId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        [Required]
        public String QrCodeId { get; set; }
        [Required]
        public long CitizenId { get; set; }
    }
}
