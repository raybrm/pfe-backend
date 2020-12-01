

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlockCovid.Models
{
    public class Citizen
    {
        //testSouf
        public long Citizen_ID { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public bool Is_Positive { get; set; }

        public virtual IList<Citizen_qr_code> Citizen_QrCode { get; set; }

    }
}
