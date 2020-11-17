using System.Threading.Tasks;
using BasePackageModule3.Areas.Admin.Models;
using BasePackageModule3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminViewModel
            {
               
                Posts = await _context.Posts.CountAsync(),
                Messages = await _context.ContactMessages.CountAsync(),
                JobSeek= await _context.FindJobs.CountAsync(),
                Service =await _context.Services.CountAsync()
            };

            return View(model);
        }
    }
}