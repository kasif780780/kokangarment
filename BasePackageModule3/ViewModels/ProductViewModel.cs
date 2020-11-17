using BasePackageModule3.Models;
using System.Collections.Generic;

namespace BasePackageModule3.ViewModels
{
    public class ProductViewModel
    {
        public IEnumerable<Item> Item { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<Coupon> Coupon { get; set; }



    }
}
