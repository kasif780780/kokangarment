using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.Models;
using BasePackageModule3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule3.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        //Get iNDEX
        public async Task<IActionResult> Index()
        {
            var subcaregories = await _context.SubCategories.Include(s => s.Category).ToListAsync();

            return View(subcaregories);
        }

        public async Task<IActionResult> Create()
        {
            SubnCatViewModel model = new SubnCatViewModel()
            {
                CategoryList = await _context.Categories.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryList = await _context.SubCategories.OrderBy(o => o.Name).Select(o => o.Name).Distinct().ToListAsync()

            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(SubnCatViewModel subnCatViewModel)
        {
            if(ModelState.IsValid)
            {
                var doesSubcategoryExist = _context.SubCategories.Include(s => s.Category)
                    .Where(s => s.Name == subnCatViewModel.SubCategory.Name
                && s.Category.Id == subnCatViewModel.SubCategory.CategoryId);
                
                
                   if(doesSubcategoryExist.Count()>0)
                {
                    //error
                    StatusMessage = "Error : Sub Category exist under " + doesSubcategoryExist.First().Category.Name + "category. Please use another name.";
                }
                else
                {
                    _context.SubCategories.Add(subnCatViewModel.SubCategory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                
                   
                
            }
            SubnCatViewModel modelvm = new SubnCatViewModel()
            {
                CategoryList = await _context.Categories.ToListAsync(),
                SubCategory = subnCatViewModel.SubCategory,
                SubCategoryList = await _context.SubCategories.OrderBy(P => P.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage

            };
            return View(modelvm);

        }

        //Edit Get
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subcategory = await _context.SubCategories.SingleOrDefaultAsync(m => m.Id == id);
            if(subcategory == null)
            {
                return NotFound();
            }
            SubnCatViewModel model = new SubnCatViewModel()
            {
                CategoryList = await _context.Categories.ToListAsync(),
                SubCategory = subcategory,
                SubCategoryList = await _context.SubCategories.OrderBy(o => o.Name).Select(o => o.Name).Distinct().ToListAsync()

            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubnCatViewModel subnCatViewModel)
        {
            if (ModelState.IsValid)
            {
                var doesSubcategoryExist = _context.SubCategories.Include(s => s.Name).Where(s => s.Name == subnCatViewModel.SubCategory.Name && s.Category.Id == subnCatViewModel.SubCategory.CategoryId);

                var subcatfromdb = await _context.SubCategories.FindAsync(id);
               // var catg = subnCatViewModel.SubCategory.CategoryId;

                subcatfromdb.Name = subnCatViewModel.SubCategory.Name;
                subcatfromdb.CategoryId = subnCatViewModel.SubCategory.CategoryId;





                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));



            }
            SubnCatViewModel modelvm = new SubnCatViewModel()
            {
                CategoryList = await _context.Categories.ToListAsync(),
                SubCategory = subnCatViewModel.SubCategory,
                SubCategoryList = await _context.SubCategories.OrderBy(P => P.Name).Select(p => p.Name).ToListAsync()

            };
            return View(modelvm);

        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in _context.SubCategories 
                                   where subCategory.CategoryId == id select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }

    

       
       

    }
}