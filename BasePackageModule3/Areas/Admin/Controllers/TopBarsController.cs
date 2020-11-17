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
    public class TopBarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TopBarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TopBars
        public async Task<IActionResult> Index()
        {
            return View(await _context.TopBars.ToListAsync());
        }

        // GET: Admin/TopBars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topBar = await _context.TopBars
                .FirstOrDefaultAsync(m => m.TopbarId == id);
            if (topBar == null)
            {
                return NotFound();
            }

            return View(topBar);
        }

        // GET: Admin/TopBars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TopBars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TopbarId,Phone,StreetAddress,City,State,Country,Zipcode,Email")] TopBar topBar)
        {
            if (ModelState.IsValid)
            {
                _context.Add(topBar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(topBar);
        }

        // GET: Admin/TopBars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topBar = await _context.TopBars.FindAsync(id);
            if (topBar == null)
            {
                return NotFound();
            }
            return View(topBar);
        }

        // POST: Admin/TopBars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TopbarId,Phone,StreetAddress,City,State,Country,Zipcode,Email")] TopBar topBar)
        {
            if (id != topBar.TopbarId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(topBar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopBarExists(topBar.TopbarId))
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
            return View(topBar);
        }

        // GET: Admin/TopBars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var topBar = await _context.TopBars
                .FirstOrDefaultAsync(m => m.TopbarId == id);
            if (topBar == null)
            {
                return NotFound();
            }

            return View(topBar);
        }

        // POST: Admin/TopBars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topBar = await _context.TopBars.FindAsync(id);
            _context.TopBars.Remove(topBar);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TopBarExists(int id)
        {
            return _context.TopBars.Any(e => e.TopbarId == id);
        }
    }
}
