using System.ComponentModel.DataAnnotations;
using BasePackageModule3.Models;

namespace BasePackageModule3.ViewModels
{
    public class ContactViewModel
    {
        public Contact _contact { get; set; }
        public string Name { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(10)]
        [MaxLength(10)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public string Company { get; set; }

        [Required]
        [MinLength(5)]
        public string Message { get; set; }

        public Footer _footer { get; set; }

    }
}
