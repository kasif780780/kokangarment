using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using BasePackageModule3.Data;
using BasePackageModule3.Helpers;
using BasePackageModule3.Models;

namespace BasePackageModule3.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class NewProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/NewProjects
        public async Task<IActionResult> Index()
        {
            return View(await _context.NewProjects.ToListAsync());
        }

        // GET: Admin/NewProjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newProject = await _context.NewProjects
                .FirstOrDefaultAsync(m => m.NewProjectId == id);
            if (newProject == null)
            {
                return NotFound();
            }

            return View(newProject);
        }

        // GET: Admin/NewProjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NewProjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile formImage, NewProject service)
        {
            if (ModelState.IsValid)
            {
                if (formImage == null)
                {
                    ModelState.AddModelError("Image", "Image is required.");
                    return View(service);
                }
                var imageName = Guid.NewGuid() + Path.GetExtension(formImage.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/NewProjects")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/NewProjects"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/NewProjects", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    formImage.CopyTo(stream);
                }

                service.Image = $"/img/Services/{imageName}";
               
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        // GET: Admin/Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.NewProjects.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        // POST: Admin/Services/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile image, NewProject service)
        {
            if (id != service.NewProjectId)
            {
                return NotFound();
            }
            var thisPost = await _context.Services.FirstAsync(p => p.Id == service.NewProjectId);

            if (thisPost == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid) return View(service);

            if (image != null)
            {
                //Set Key Name
                var imageName = Guid.NewGuid() + Path.GetExtension(image.FileName);

                if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Services")))
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Services"));
                }

                //Get url To Save
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Services", imageName);

                await using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                thisPost.Image = $"/img/Services/{imageName}";

            }
            try
            {
                thisPost.Slug = UrlHelper.GetFriendlyTitle(service.Title);
                thisPost.Content = service.Description;
                thisPost.Title = service.Title;
              
                _context.Update(thisPost);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewProjectExists(service.NewProjectId))
                {
                    return NotFound();
                }

                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/NewProjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newProject = await _context.NewProjects
                .FirstOrDefaultAsync(m => m.NewProjectId == id);
            if (newProject == null)
            {
                return NotFound();
            }

            return View(newProject);
        }

        // POST: Admin/NewProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var newProject = await _context.NewProjects.FindAsync(id);
            _context.NewProjects.Remove(newProject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewProjectExists(int id)
        {
            return _context.NewProjects.Any(e => e.NewProjectId == id);
        }
    }
}
