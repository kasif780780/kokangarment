using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BasePackageModule3.Models
{
    public class OrderDetails
    {
        [Key]
        public int  Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual OrderHeader OrderHeader { get; set; }

        [Required]
        public int ItemId { get; set; }

        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }

        public int Count { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        public double Price { get; set; }

    }
}
