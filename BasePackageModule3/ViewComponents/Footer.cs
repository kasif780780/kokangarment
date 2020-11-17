using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace BasePackageModule3.ViewComponents
{
    [ViewComponent(Name = "Footer")]
    public class Footer : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public Footer(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var snewproject = new Models.Footer();
            if (await _context.Footers.AnyAsync())
            {
                snewproject = await _context.Footers.FirstOrDefaultAsync();
            }

           
            List<Models.Service> courses = await _context.Services.ToListAsync();
            NavViewModel model = new NavViewModel
            {

                _service = courses,

                _footer = snewproject,


            };

            return View("Index", model);
        }
    }
}