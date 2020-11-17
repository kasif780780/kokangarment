using System.Collections.Generic;
using BasePackageModule1.Models;

namespace BasePackageModule1.ViewModels
{
    public  class SubnCatViewModel
    { 
        public IEnumerable<Category> CategoryList { get; set; }
        public SubCategory SubCategory { get; set; }
        public List<string> SubCategoryList { get; set; }
        public string StatusMessage { get; set; }
    }
}
