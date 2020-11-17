using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule1.Data;
using BasePackageModule1.Models;
using BasePackageModule1.Models.Menu;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule1.Areas.Unibase.Controllers
{
    [Area("Unibase")]
    public class MenusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Menus
        public async Task<IActionResult> Index()
        {
            return View(await _context.Menus.OrderBy(a => a.Order).ToListAsync());
        }

        // GET: Admin/Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .Include(a => a.MenuProducts)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: Admin/Menus/Create
        public IActionResult Create()
        {
            ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");

            return View();
        }

        // POST: Admin/Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<int> products, Menu menu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(menu);
                await _context.SaveChangesAsync();

                if (products != null)
                {
                    foreach (var product in products)
                    {
                        await _context.AddAsync(entity: new MenuProduct
                        {
                            ProductId = product,
                            MenuId = menu.Id
                        });
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Products"] = new SelectList(_context.Products, "Id", "Name");

            return View(menu);
        }

        // GET: Admin/Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }

            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name",
                _context.MenuProducts.Where(a => a.MenuId == id).Select(e => e.ProductId).ToArray());
            return View(menu);
        }

        // POST: Admin/Menus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, List<int> products, Menu menu)
        {
            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menu);

                    _context.MenuProducts.RemoveRange(await _context.MenuProducts.Where(e => e.MenuId == menu.Id).ToListAsync());

                    products?.ForEach(p =>
                    {
                        _context.Add(new MenuProduct()
                        {
                            ProductId = p,
                            MenuId = menu.Id
                        });
                    });


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
                //_context.MenuProducts.Where(a => a.MenuId == id).Select(a => a.ProductId).ToList()
            }

            ViewData["Products"] = new MultiSelectList(_context.Products, "Id", "Name",
                _context.MenuProducts.Where(a => a.MenuId == id).Select(e => e.ProductId).ToArray());


            return View(menu);
        }

        // GET: Admin/Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Admin/Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ResetMenu()
        {
            var menus = await _context.Menus.ToListAsync();

            _context.RemoveRange(menus);

            await _context.SaveChangesAsync();

            await _context.AddRangeAsync(new List<Menu>
            {
                new Menu
                {
                    Name = "Home",
                    Url = "/",
                    Order = 1
                },
                new Menu
                {
                    Name = "Products",
                    Url = "/Products",
                    Order = 2
                },
                new Menu
                {
                    Name = "Latest Updates",
                    Url = "/blog",
                    Order = 3
                },
                new Menu
                {
                    Name = "Gallery",
                    Url = "/Gallery",
                    Order = 4
                },
                new Menu
                {
                    Name = "Contact us",
                    Url = "/Home/Contact",
                    Order = 5
                }
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }
    }
}
