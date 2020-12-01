
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlockCovid.Models
{
    public class Participant
    {

        public Participant()
        {
            QrCode = new HashSet<QrCode>();
        }

        public long ParticipantID { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public ParticipantType Participant_Type { get; set; }
        public virtual ICollection<QrCode> QrCode { get; set; }
    }

    public enum ParticipantType
    {
        Doctor,
        Establishment
    }
}
