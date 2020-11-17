using System.Collections.Generic;
using BasePackageModule1.Models;

namespace BasePackageModule1.ViewModels
{
    public class ProductViewModel
    {
        public IEnumerable<Product> Item { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<Coupon> Coupon { get; set; }



    }
}
