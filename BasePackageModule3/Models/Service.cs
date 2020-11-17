using System;
using System.ComponentModel.DataAnnotations;

namespace BasePackageModule3.Models
{
    public class Service
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        public string Slug { get; set; }
        [Display(Name = "Meta description")]
        public string MetaDescription { get; set; }
        [Display(Name = "Meta keywords")]
        public string MetaKeywords { get; set; }


        public string Image { get; set; }

        [Required]
        public string Content { get; set; }

        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }

        public Service()
        {
            CreatedAt = DateTime.Now;
        }
    }
}
