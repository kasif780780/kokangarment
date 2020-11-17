using System.Linq;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule3.ViewComponents
{
    [ViewComponent(Name = "TopBar")]
    public class TopBar : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public TopBar(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
           
            if(_context.Footers.Count()>0)
            {
                var topbars = await _context.Footers.FirstOrDefaultAsync();
                return View("Index", topbars);
            }
            return View("Index", new Models.Footer());
        }
    }
}