using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlockCovid.Models.Dto
{
    public class ParticipantConnexionDto
    {
        [EmailAddress]
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
