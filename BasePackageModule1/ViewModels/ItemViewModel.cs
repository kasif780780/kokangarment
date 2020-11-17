using System.Collections.Generic;
using BasePackageModule1.Models;

namespace BasePackageModule1.ViewModels
{
    public class ItemViewModel
    {
        public Product Item { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<SubCategory> SubCategory { get; set; }
    }
}
