using System;
using System.ComponentModel.DataAnnotations;

namespace BasePackageModule3.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        [Required]
        public int CourseId { get; set; }

      
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public string Location { get; set; }

    }
}
