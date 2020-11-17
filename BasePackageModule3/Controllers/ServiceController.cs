using System.Linq;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BasePackageModule3.Controllers
{
    public class ServiceController : Controller
    {
        private readonly ILogger<ServiceController> _logger;
        private readonly ApplicationDbContext _context;
        public ServiceController(ILogger<ServiceController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Services.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var footer = await _context.Services
                .FirstOrDefaultAsync(m => m.Id == id);
            if (footer == null)
            {
                return NotFound();
            }
            ViewBag.Posts = await _context.Services.OrderByDescending(a => a.Id).ToListAsync();
            return View(footer);
        }
    }
}