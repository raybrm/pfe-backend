using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models.Dto
{
    public class ScanQrCodeDto
    {
        [Required]
        public String QrCode { get; set; }
        [Required]
        public long citizen { get; set; }
    }
}
