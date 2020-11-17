using BasePackageModule3.Models;
using System.Collections.Generic;

namespace BasePackageModule3.ViewModels
{
    public class OrderDetailsCart
    {
         public List<ShoppingCart> listCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
