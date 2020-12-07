using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models.Dto
{
    public class ParticipantDto
    {
        [EmailAddress]
        [Required]
        public string Login { get; set; }
        [Required]
        [MaxLength(20), MinLength(5)]
        public string Password { get; set; }
        [Required]
        //[Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public ParticipantType? Participant_Type { get; set; }
    }
}
