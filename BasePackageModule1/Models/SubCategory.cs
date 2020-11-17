using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasePackageModule1.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }

        [Display(Name="Sub Category")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

    }
}
