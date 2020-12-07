using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models.Dto
{
    public class VerifyParticipant
    {
        [Required]
        public string Token { get; set; }
        public ParticipantType Role { get; set; }
    }
}
