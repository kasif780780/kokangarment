using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BasePackageModule1.Data;
using BasePackageModule1.Models;
using BasePackageModule1.Utility;
using BasePackageModule1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace BasePackageModule1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string category, string query, int minPrice, int maxPrice, int pageindex = 1, int PageSize = 6, string orderby = null)
        {

            IQueryable<Product> products = from s in _context.Products
                                           select s;

            if (category != null)
            {
                products = products.Where(c => c.Category.Name == category);
            }

            if (query != null)
            {
                products = products.Where(s => s.Name.Contains(query));
            }

            switch (orderby)
            {
                //case "Date":
                //    products = products.OrderBy(s => s.CreatedAt);
                //    break;
                case "price":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "price-desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                default:
                    products = products.OrderBy(s => s.Name);
                    break;
            }

            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem() { Value="6", Text= "6" },
                new SelectListItem() { Value="12", Text= "12" },
                new SelectListItem() { Value="24", Text= "24" },
                new SelectListItem() { Value="48", Text= "48" },
                new SelectListItem() { Value="100", Text= "100" },
            };
            if (minPrice > 0)
            {
                products = products.Where(p => p.Price >= minPrice);
                ViewBag.maxPrice = minPrice;
            }

            if (maxPrice > 0)
            {
                products = products.Where(p => p.Price <= maxPrice);
                ViewBag.minPrice = maxPrice;
            }

          
            ViewBag.orderby = orderby;
            ViewBag.query = query;

            return View(
                await products
                    .ToPagedListAsync(pageNumber: pageindex, pageSize: PageSize));


        }

        [HttpGet]
        [Route("search/{min}/{max}")]
        public IActionResult Search( double min, double max)
        {
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Item =  _context.Products.Include(m => m.Category).Include(m => m.SubCategory).ToList(),
                Category =  _context.Categories.ToList(),
                Coupon = _context.Coupons.Where(c => c.IsActive == true).ToList()
            };
            return new JsonResult(productViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .Include(c => c.Images)
                .Where(m => m.Id == id).FirstOrDefaultAsync();
           
            return View(product);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cart)
        {
            cart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _context.ShoppingCarts.Where(c => c.ApplicationUserId == cart.ApplicationUserId && c.ItemId == cart.ItemId).FirstOrDefaultAsync();
                if (cartFromDb == null)
                {
                    await _context.AddAsync(cart);

                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + cart.Count;

                }
                await _context.SaveChangesAsync();
                var count = _context.ShoppingCarts.Where(c => c.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

                return RedirectToAction("Index");
            }
            else
            {
                var itemfromdb = await _context.Products.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == cart.ItemId).FirstOrDefaultAsync();
                ShoppingCart cartobj = new ShoppingCart()
                {
                    Item = itemfromdb,
                    ItemId = itemfromdb.Id
                };

                return View(cartobj);
            }
        }

        [Route("Show/{Id?}/{menuName?}")]
        public async Task<IActionResult> Show(int? id, string? menuName)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.Include(a => a.MenuProducts).ThenInclude(a => a.Product).FirstOrDefaultAsync(a => a.Id == id);
            if (menu != null)
            {
                var products = await menu.MenuProducts.Where(a => a.MenuId == menu.Id).Select(a => a.Product).ToListAsync();
                return View(products);
            }

            return NotFound();


        }
    }
}