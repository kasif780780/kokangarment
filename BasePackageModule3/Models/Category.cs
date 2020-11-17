using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BasePackageModule3.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; }

    }
}
