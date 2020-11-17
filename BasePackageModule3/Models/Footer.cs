using System.ComponentModel.DataAnnotations;

namespace BasePackageModule3.Models
{
    public class Footer
    {

        public int id  { get; set; }
        public string Phone { get; set; }
        [MaxLength(61)]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
        public string Email { get; set; }
        public string WhatsApp { get; set; }
        public string Facebook { get; set; }
        public string LinkedIn { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }
        public string FacebookLikePage { get; set; }

        public string Tagline { get; set; }
        public string Copyright { get; set; }



    }
}
