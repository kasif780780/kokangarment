using System.Collections.Generic;
using BasePackageModule3.Models;

namespace BasePackageModule3.ViewModels
{
    public  class SubnCatViewModel
    { 
        public IEnumerable<Category> CategoryList { get; set; }
        public SubCategory SubCategory { get; set; }
        public List<string> SubCategoryList { get; set; }
        public string StatusMessage { get; set; }
    }
}
