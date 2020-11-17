using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace BasePackageModule3.ViewComponents
{
    [ViewComponent(Name = "Favicon")]
    public class Favicon:ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public Favicon(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var snewproject = new Models.Logo();

            if(await _context.Logos.AnyAsync())
            {
             snewproject = await _context.Logos.FirstOrDefaultAsync();
            }
         

            NavViewModel model = new NavViewModel
            {

                

                _Logo = snewproject,


            };

            return View("Index", model);
        }
    }
}
