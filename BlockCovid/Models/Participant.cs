
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlockCovid.Models
{
    public class Participant
    {
        public long Participant_ID { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public ParticipantType Participant_Type { get; set; }
        public virtual IList<Qr_code> QrCode { get; set; }
    }

    public enum ParticipantType
    {
        Doctor,
        Establishment
    }
}
