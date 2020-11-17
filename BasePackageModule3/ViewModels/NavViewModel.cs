using BasePackageModule3.Models;
using System.Collections.Generic;

namespace BasePackageModule3.ViewModels
{
    public class NavViewModel
    {
        public List<Service> _service { get; set; }
        public List<NewProject> _newProject { get; set; }
        public List<Page> _more { get; set; }
        public List<Course> _course { get; set; }
        public Logo _Logo { get; set; }

        public Footer _footer { get; set; }

        public List<Category> Categories { get; set; }
    }
}
