using System.Collections.Generic;
using BasePackageModule3.Models;

namespace BasePackageModule3.ViewModels
{
    public class ItemViewModel
    {
        public Item Item { get; set; }
        public IEnumerable<Category> Category { get; set; }
        public IEnumerable<SubCategory> SubCategory { get; set; }
    }
}
