using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BasePackageModule1.Models;
using BasePackageModule2.Data;

namespace BasePackageModule1.Areas.CatBase.Controllers
{
    [Area("CatBase")]
    public class PincodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PincodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CatBase/Pincodes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pincodes.ToListAsync());
        }

        // GET: CatBase/Pincodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pincode = await _context.Pincodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pincode == null)
            {
                return NotFound();
            }

            return View(pincode);
        }

        // GET: CatBase/Pincodes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CatBase/Pincodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code")] Pincode pincode)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pincode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pincode);
        }

        // GET: CatBase/Pincodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pincode = await _context.Pincodes.FindAsync(id);
            if (pincode == null)
            {
                return NotFound();
            }
            return View(pincode);
        }

        // POST: CatBase/Pincodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code")] Pincode pincode)
        {
            if (id != pincode.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pincode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PincodeExists(pincode.Id))
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
            return View(pincode);
        }

        // GET: CatBase/Pincodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pincode = await _context.Pincodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pincode == null)
            {
                return NotFound();
            }

            return View(pincode);
        }

        // POST: CatBase/Pincodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pincode = await _context.Pincodes.FindAsync(id);
            _context.Pincodes.Remove(pincode);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PincodeExists(int id)
        {
            return _context.Pincodes.Any(e => e.Id == id);
        }
    }
}
