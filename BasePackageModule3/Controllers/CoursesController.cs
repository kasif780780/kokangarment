#nullable enable
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule3.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        [Route("Courses")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Courses.ToListAsync());
        }

        // GET: Courses/Details/5
        [Route("Courses/{Id?}/{slug?}")]
        public async Task<IActionResult> Details(int? id, string? slug)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }


        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }

    }
}