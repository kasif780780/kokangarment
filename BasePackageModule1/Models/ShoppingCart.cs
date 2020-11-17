using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasePackageModule1.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }

        public int ItemId { get; set; }

        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [NotMapped]
        [ForeignKey("ItemId")]
        public virtual Product Item { get; set; }

        [Range(1, int.MaxValue, ErrorMessage ="Please Enter a Value greater than equal to {1}")]
        public int Count { get; set; }
    }
}
