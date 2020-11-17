using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using BasePackageModule3.Data;
using BasePackageModule3.Models;

namespace BasePackageModule3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class FindJobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FindJobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/FindJobs
        public async Task<IActionResult> Index()
        {
            return View(await _context.FindJobs.ToListAsync());
        }

        // GET: Admin/FindJobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var findJob = await _context.FindJobs
                .FirstOrDefaultAsync(m => m.FindJobId == id);
            if (findJob == null)
            {
                return NotFound();
            }

            return View(findJob);
        }

        // GET: Admin/FindJobs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/FindJobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
         [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile formImage, FindJob post)
        {
            if (ModelState.IsValid)
            {
                if (formImage == null)
                {
                    ModelState.AddModelError("Image", "Image is required.");
                    return View(post);
                }
                var imageName = Guid.NewGuid() + Path.GetExtension(formImage.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Resume/resume")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Resume/resume"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Resume/resume", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    formImage.CopyTo(stream);
                }

              

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Admin/FindJobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var findJob = await _context.FindJobs.FindAsync(id);
            if (findJob == null)
            {
                return NotFound();
            }
            return View(findJob);
        }

        // POST: Admin/FindJobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FindJobId,FullName,Mobile,Email,City,Location,Age,Gender,Qualification,SchoolMedium,SpeakEnglish,Experience,JobRole,Resume")] FindJob findJob)
        {
            if (id != findJob.FindJobId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(findJob);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FindJobExists(findJob.FindJobId))
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
            return View(findJob);
        }

        // GET: Admin/FindJobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var findJob = await _context.FindJobs
                .FirstOrDefaultAsync(m => m.FindJobId == id);
            if (findJob == null)
            {
                return NotFound();
            }

            return View(findJob);
        }

        // POST: Admin/FindJobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var findJob = await _context.FindJobs.FindAsync(id);
            _context.FindJobs.Remove(findJob);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FindJobExists(int id)
        {
            return _context.FindJobs.Any(e => e.FindJobId == id);
        }
    }
}
