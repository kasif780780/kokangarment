using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace BasePackageModule3.ViewComponents
{
    [ViewComponent(Name = "Seo")]
    public class Seo: ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public Seo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var courses = new Models.Seo();
            if (await _context.Seos.AnyAsync())
            {
                courses = await _context.Seos.FirstOrDefaultAsync();
            }
          
          
            return View("Index", courses);
        }
    }
}
