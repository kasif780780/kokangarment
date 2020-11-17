using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BasePackageModule3.Data;
using BasePackageModule3.Models;

namespace BasePackageModule3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SeosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Seos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Seos.ToListAsync());
        }

        // GET: Admin/Seos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seo = await _context.Seos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seo == null)
            {
                return NotFound();
            }

            return View(seo);
        }

        // GET: Admin/Seos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Seos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MetaTitle,MetaDiscription,CanonicalTag,OgTitle,OgDescription,OgUrl,OgSiteName,MetaTwitter,MetaTwitterDescription,TwitterTitle,MsValidate,GSiteVerivation")] Seo seo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(seo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(seo);
        }

        // GET: Admin/Seos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seo = await _context.Seos.FindAsync(id);
            if (seo == null)
            {
                return NotFound();
            }
            return View(seo);
        }

        // POST: Admin/Seos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MetaTitle,MetaDiscription,CanonicalTag,OgTitle,OgDescription,OgUrl,OgSiteName,MetaTwitter,MetaTwitterDescription,TwitterTitle,MsValidate,GSiteVerivation")] Seo seo)
        {
            if (id != seo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeoExists(seo.Id))
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
            return View(seo);
        }

        // GET: Admin/Seos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seo = await _context.Seos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seo == null)
            {
                return NotFound();
            }

            return View(seo);
        }

        // POST: Admin/Seos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seo = await _context.Seos.FindAsync(id);
            _context.Seos.Remove(seo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeoExists(int id)
        {
            return _context.Seos.Any(e => e.Id == id);
        }
    }
}
