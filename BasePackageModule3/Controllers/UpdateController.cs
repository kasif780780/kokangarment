using System;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BasePackageModule3.Controllers
{
    public class UpdateController : Controller
    {
        private readonly ILogger<UpdateController> _logger;
        private readonly ApplicationDbContext _context;
        public UpdateController(ILogger<UpdateController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index(
     string sortOrder,
     string currentFilter,
     string searchString,
     int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var students = from s in _context.Updates
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.Title.Contains(searchString)
                                       || s.Slug.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Title);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.Slug);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.Title);
                    break;
                default:
                    students = students.OrderBy(s => s.Title);
                    break;
            }
            int pageSize = 10;
            return View(await PaginatedList<Update>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var footer = await _context.Updates
                .FirstOrDefaultAsync(m => m.Id == id);
            if (footer == null)
            {
                return NotFound();
            }
            ViewBag.Updates = await _context.Updates.OrderByDescending(a => a.CreatedDate).Take(4).ToListAsync();
            return View(footer);
        }
    }
}