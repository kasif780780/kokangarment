using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.Models;
using BasePackageModule3.Utility;
using BasePackageModule3.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ItemViewModel ItemViewModel { get; set; }

        public ItemsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;

            ItemViewModel = new ItemViewModel()
            {
                Category = _context.Categories,
                Item = new Item()
            };
        }
        public async Task<IActionResult> Index()
        {
            var items = await _context.Items.Include(c => c.Category).Include(c => c.SubCategory).ToListAsync();
            return View(items);
        }


        //Get Create
        public IActionResult Create()
        {
            return View(ItemViewModel);
        }


        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            ItemViewModel.Item.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (!ModelState.IsValid)
            {
                return View(ItemViewModel);
            }
            _context.Items.Add(ItemViewModel.Item);
            await _context.SaveChangesAsync();

            //Image Saving

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var ItemFromDb = await _context.Items.FindAsync(ItemViewModel.Item.Id);
            if (files.Count > 0)
            {
                //Files have been Uploaded
                var uploads = Path.Combine(webRootPath, "img");
                var extension = Path.GetExtension(files[0].FileName);
                using (var fileStream = new FileStream(Path.Combine(uploads, ItemViewModel.Item.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);

                }
                ItemFromDb.Image = @"\img\" + ItemViewModel.Item.Id + extension;
            }
            else
            {
                // no files was Uploaded so use default
                var uploads = Path.Combine(webRootPath, @"img\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\img\" + ItemViewModel.Item.Id + ".png");
                ItemFromDb.Image = @"\img\" + ItemViewModel.Item.Id + ".png";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        //Get Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ItemViewModel.Item = await _context.Items.Include(c => c.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            ItemViewModel.SubCategory = await _context.SubCategories.Where(s => s.CategoryId == ItemViewModel.Item.CategoryId).ToListAsync();
            if (ItemViewModel.Item == null)
            {
                return NotFound();
            }
            return View(ItemViewModel);
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ItemViewModel.Item.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if (!ModelState.IsValid)
            {
                ItemViewModel.SubCategory = await _context.SubCategories.Where(s => s.CategoryId == ItemViewModel.Item.CategoryId).ToListAsync();
                return View(ItemViewModel);
            }


            //Image Saving

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var ItemFromDb = await _context.Items.FindAsync(ItemViewModel.Item.Id);
            if (files.Count > 0)
            {
                //Files have been Uploaded
                var uploads = Path.Combine(webRootPath, "img");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original File
                var imagepath = Path.Combine(webRootPath, ItemFromDb.Image.Trim('\\'));
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }

                using (var fileStream = new FileStream(Path.Combine(uploads, ItemViewModel.Item.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);

                }
                ItemFromDb.Image = @"\img\" + ItemViewModel.Item.Id + extension_new;
            }

            ItemFromDb.Name = ItemViewModel.Item.Name;
            ItemFromDb.Price = ItemViewModel.Item.Price;
            ItemFromDb.Description = ItemViewModel.Item.Description;
            ItemFromDb.CategoryId = ItemViewModel.Item.CategoryId;
            ItemFromDb.SubCategoryId = ItemViewModel.Item.SubCategoryId;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        // GET: Admin/Services/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: Admin/Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Items.FindAsync(id);
            _context.Items.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }
    }
}