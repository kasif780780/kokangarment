using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BasePackageModule1.Areas.CatBase.ViewModels
{
    public class Colors
    {
        [Required]
        [DisplayName("Primary Color")]
        public string PrimaryColor { get; set; }    
    }
}
