using System.ComponentModel.DataAnnotations;

namespace BasePackageModule3.Models
{
    public class FindJob
    {
        public int FindJobId { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string Email { get; set; }


        public string City { get; set; }
        public string Location { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string Qualification { get; set; }
        public string SchoolMedium { get; set; }
        public string SpeakEnglish { get; set; }
        public string Experience { get; set; }
        public string JobRole { get; set; }
        public string Resume { get; set; }





    }
}
