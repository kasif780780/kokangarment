using System.Linq;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BasePackageModule3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BannerBottomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BannerBottomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/BannerBottoms
        public async Task<IActionResult> Index()
        {
            return View(await _context.BannerBottoms.ToListAsync());
        }

        // GET: Admin/BannerBottoms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bannerBottom = await _context.BannerBottoms
                .FirstOrDefaultAsync(m => m.BannerBottomId == id);
            if (bannerBottom == null)
            {
                return NotFound();
            }

            return View(bannerBottom);
        }

        // GET: Admin/BannerBottoms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/BannerBottoms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BannerBottomId,ShortDesction")] BannerBottom bannerBottom)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bannerBottom);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bannerBottom);
        }

        // GET: Admin/BannerBottoms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bannerBottom = await _context.BannerBottoms.FindAsync(id);
            if (bannerBottom == null)
            {
                return NotFound();
            }
            return View(bannerBottom);
        }

        // POST: Admin/BannerBottoms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BannerBottomId,ShortDesction")] BannerBottom bannerBottom)
        {
            if (id != bannerBottom.BannerBottomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bannerBottom);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BannerBottomExists(bannerBottom.BannerBottomId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bannerBottom);
        }

        // GET: Admin/BannerBottoms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bannerBottom = await _context.BannerBottoms
                .FirstOrDefaultAsync(m => m.BannerBottomId == id);
            if (bannerBottom == null)
            {
                return NotFound();
            }

            return View(bannerBottom);
        }

        // POST: Admin/BannerBottoms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bannerBottom = await _context.BannerBottoms.FindAsync(id);
            _context.BannerBottoms.Remove(bannerBottom);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BannerBottomExists(int id)
        {
            return _context.BannerBottoms.Any(e => e.BannerBottomId == id);
        }
    }
}
