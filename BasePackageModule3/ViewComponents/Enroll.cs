using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule3.ViewComponents
{
    [ViewComponent(Name = "Enroll")]
    public class Enroll : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public Enroll(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var courses = await _context.Courses.ToListAsync();
            ViewData["courses"] = courses;
            return View("Index", new Enrollment());
        }
    }
}