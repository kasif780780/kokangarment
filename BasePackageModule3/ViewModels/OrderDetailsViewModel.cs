using System.Collections.Generic;
using BasePackageModule3.Models;

namespace BasePackageModule3.ViewModels
{
    public class OrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}
