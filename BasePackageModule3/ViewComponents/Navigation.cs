using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.Models;
using BasePackageModule3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule3.ViewComponents
{
    [ViewComponent(Name = "Navigation")]
    public class Navigation :ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public Navigation(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var logo = await _context.Logos.AnyAsync() 
                ? await _context.Logos.FirstOrDefaultAsync() 
                : new Logo();


            var footer = await _context.Footers.AnyAsync()
                ? await _context.Footers.FirstOrDefaultAsync()
                : new Models.Footer();


            List<Service> services = await _context.Services.OrderBy(o=>o.Order).ToListAsync();
            List<Course> courses = await _context.Courses.OrderBy(s => s.Order).ToListAsync();
            List<Page> page = await _context.Pages.OrderBy(o => o.Order).ToListAsync();
            List<NewProject> product = await _context.NewProjects.OrderByDescending(s => s.NewProjectId).ToListAsync();
            var categories = await _context.Categories.Include(s => s.SubCategories).OrderByDescending(k => k.SubCategories.Count).ToListAsync();
         

            NavViewModel model = new NavViewModel
            {

                _service = services,
                _Logo = logo,
                _more = page,
                _newProject =product,
                _course = courses,
                Categories = categories,
                _footer = footer
            };

            return View("Index", model);
        }
    }
}
