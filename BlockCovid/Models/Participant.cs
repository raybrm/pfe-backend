using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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
        public ParticipantType? Participant_Type { get; set; }

        //Navigation property
        public virtual ICollection<QrCode> QrCode { get; set; }
    }

    
    public enum ParticipantType
    {
        [EnumMember(Value = "doctor")]
        Doctor,
        [EnumMember(Value = "establishment")]
        Establishment
    }
}
