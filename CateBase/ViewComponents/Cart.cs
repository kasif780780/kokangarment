using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.Extensions;
using BasePackageModule2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule1.ViewComponents
{

    [ViewComponent(Name = "Cart")]
    public class Cart : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Cart(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            var cart = await _context.Carts.Include(p => p.Product).ThenInclude(i => i.Images).Where(u => u.UserId == user.Id).ToListAsync();
            return View(cart);
        }
       
    }
}
