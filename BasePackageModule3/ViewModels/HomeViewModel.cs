using System.Collections.Generic;
using BasePackageModule3.Models;

namespace BasePackageModule3.ViewModels
{
    public class HomeViewModel
    {
        public List<SliderImage> SliderImages { get; set; }

        public AboutUs _AboutUs { get; set; }
      
        public string Banner { get; set; }
        public List<NewProject> _NewProject { get; set; }
        public List<Post> _Update { get; set; }
        public List<Image> _Image { get; set; }
        public List<Service> _service { get; set; }
        public List<Item> _item { get; set; }
        public List<Course> _corses { get; set; }

        public Service _ssservice { get; set; }
        public Course _sscourse { get; set; }
        public NewProject _snewproject { get; set; }
        public Post _supdate { get; set; }
        public Footer _footer { get; set; }
        public Item _sitem { get; set; }

    }
}
