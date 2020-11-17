using System.Threading.Tasks;
using BasePackageModule1.Areas.Unibase.Models;
using BasePackageModule1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule1.Areas.Unibase.Controllers
{
    [Area("Unibase")]
    [Authorize(Roles = "Admin")]

    public class UnibaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UnibaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminViewModel
            {
                Posts = await _context.Posts.CountAsync(),
                Messages = await _context.ContactMessages.CountAsync(),
            };

            return View(model);
        }
    }
}