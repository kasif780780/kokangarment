using System.Threading.Tasks;
using BasePackageModule2.Areas.CatBase.ViewModels;
using BasePackageModule2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule2.Areas.CatBase.Controllers
{
    [Area("CatBase")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class CatBaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatBaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminViewModel
            {
                Posts = await _context.Posts.CountAsync(),
                Messages = await _context.ContactMessages.CountAsync(),
                Products = await _context.Products.CountAsync(),
                Categories = await _context.Categories.CountAsync()
            };

            return View(model);
        }
    }
}