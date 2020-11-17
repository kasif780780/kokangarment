using System.Linq;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FootersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FootersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Footers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Footers.ToListAsync());
        }

        // GET: Admin/Footers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var footer = await _context.Footers
                .FirstOrDefaultAsync(m => m.id == id);
            if (footer == null)
            {
                return NotFound();
            }

            return View(footer);
        }

        // GET: Admin/Footers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Footers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Phone,StreetAddress,City,State,Country,Zipcode,Email,WhatsApp,Facebook,LinkedIn,Instagram,Twitter,FacebookLikePage,Copyright,Tagline")] Footer footer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(footer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(footer);
        }

        // GET: Admin/Footers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var footer = await _context.Footers.FindAsync(id);
            if (footer == null)
            {
                return NotFound();
            }
            return View(footer);
        }

        // POST: Admin/Footers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Phone,StreetAddress,City,State,Country,Zipcode,Email,WhatsApp,Facebook,LinkedIn,Instagram,Twitter,FacebookLikePage,Copyright,Tagline")] Footer footer)
        {
            if (id != footer.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(footer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FooterExists(footer.id))
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
            return View(footer);
        }

        // GET: Admin/Footers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var footer = await _context.Footers
                .FirstOrDefaultAsync(m => m.id == id);
            if (footer == null)
            {
                return NotFound();
            }

            return View(footer);
        }

        // POST: Admin/Footers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var footer = await _context.Footers.FindAsync(id);
            _context.Footers.Remove(footer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FooterExists(int id)
        {
            return _context.Footers.Any(e => e.id == id);
        }
    }
}
