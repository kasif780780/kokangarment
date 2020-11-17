using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace BasePackageModule1.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
       
        [DisplayName("Main Image")]
        public string Image { get; set; }

        public ICollection<ProductImage> Images { get; set; }

        public string Description { get; set; }
        

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "Sub Category")]
        public int? SubCategoryId { get; set; }

        [DisplayName("Sub Category")]
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        public double? Price { get; set; }

        [DisplayName("Product Tags")]
        public string ProductTags { get; set; }

        public bool Status { get; set; }

    }
}
