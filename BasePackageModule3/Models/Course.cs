using System.ComponentModel.DataAnnotations;

namespace BasePackageModule3.Models
{
    public class Course
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Display(Name = "Meta description")]
        public string MetaDescription { get; set; }
        [Display(Name = "Meta keywords")]
        public string MetaKeywords { get; set; }

        [Required]
        public string Category { get; set; }
        public string Image { get; set; }

        [Required]
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }       
        
        [Required] 
        public string Description { get; set; }

        public int Order { get; set; }
    }
}