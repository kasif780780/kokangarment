#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BasePackageModule3.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly ApplicationDbContext _context;
        public BlogController(ILogger<BlogController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [Route("blog")]
        public async Task<IActionResult> Index()
        {
            List<Post> posts = await _context.Posts.OrderBy(o => o.CreatedAt).ToListAsync();

            return View(posts);
        }

        [Route("blog/{Id?}/{slug?}")]
        public async Task<IActionResult> Details(int? id, string? slug)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            ViewBag.Posts = await _context.Posts.OrderByDescending(a => a.CreatedAt).Take(4).ToListAsync();

            return View(post);
        }

    }
}