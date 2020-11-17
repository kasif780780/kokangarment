using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BasePackageModule1.Data;
using BasePackageModule1.Extensions;
using BasePackageModule1.Helpers;
using BasePackageModule1.Models;
using BasePackageModule1.Models.Menu;
using BasePackageModule1.Utility;
using BasePackageModule1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BasePackageModule1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IEmailConfiguration _emailConfiguration;
        private readonly ITemplateHelper _templateHelper;
        private readonly UserManager<IdentityUser> _userManager;
       
        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<HomeController> logger, ApplicationDbContext context, IEmailConfiguration emailConfiguration, IEmailService emailService, ITemplateHelper templateHelper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _context = context;
            _emailConfiguration = emailConfiguration;
            _emailService = emailService;
            _templateHelper = templateHelper;
        }

        public async Task<IActionResult> Index()
        {
            //ApplicationUser user = new ApplicationUser
            //{
            //    Email = "manage@safesword.in",
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //    UserName = "manage@safesword.in",
            //    Name = "Salman Khan"
            //};


            //var adminUser = await _userManager.CreateAsync(user, "123@safesword");

            //if (adminUser.Succeeded)
            //{

            //    await _roleManager.CreateAsync(new IdentityRole
            //    {
            //        Name = "Admin"
            //    });
            //    await _roleManager.CreateAsync(new IdentityRole
            //    {
            //        Name = "User"
            //    });

            //    await _userManager.AddToRoleAsync(user, "Admin");
            //}

            //await _context.SaveChangesAsync();

            var about = _context.AboutUs.Any() ? await _context.AboutUs.FirstOrDefaultAsync() : new AboutUs();
            
          

            var banner = await _context.Banners.FirstOrDefaultAsync();
             

            var images = await _context.SliderImages.ToListAsync();
            var update = await _context.Posts.OrderByDescending(s => s.Id).Take(4).ToListAsync();
            var gallery = await _context.Images.OrderByDescending(s => s.Id).Take(8).ToListAsync();
            var supdate = await _context.Posts.FirstOrDefaultAsync();
            var businessProfile = await _context.BusinessProfile.FirstOrDefaultAsync();
            var items = await _context.Products.OrderByDescending(s => s.Id).Take(4).ToListAsync();
            var menus = await _context.Menus.Where(a => a.ShowOnHomeScreen).Include(a => a.MenuProducts).ThenInclude(a => a.Product).ToListAsync();

            HomeViewModel model = new HomeViewModel
            {
                SliderImages = images,
                _AboutUs = about,
                Updates = update,
                _Image = gallery,
                _supdate =supdate,
                BusinessProfile = businessProfile,
                _items = items,
                Menus = menus,
            };
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim!=null)
            {
                var cnt = _context.ShoppingCarts.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }
            return View(model);
        }
        public IActionResult GetJob()
        {
            return View();
        }


        public IActionResult Thankyou()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> AboutUs(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var footer = await _context.AboutUs
                .FirstOrDefaultAsync(m => m.AboutUsId == id);
            if (footer == null)
            {
                return NotFound();
            }

            return View(footer);
        }
       
      
       

        public IActionResult Contact()
        {
            var footer = new BusinessProfile();


          
            if (_context.BusinessProfile.Any())
            {
                footer = _context.BusinessProfile.FirstOrDefault();
            }
            var item = _context.Contacts.FirstOrDefault();
            ContactViewModel model = new ContactViewModel()
            {
                _contact = item,
                BusinessProfile = footer

                
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMessage contactMessage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contactMessage);
                await _context.SaveChangesAsync();

                EmailService.EmailMessage emailMessage = new EmailService.EmailMessage
                {
                    FromAddress = new EmailService.EmailAddress
                    {
                        Name = contactMessage.Name,
                        Address = contactMessage.Email
                    },
                    ToAddress = new EmailService.EmailAddress
                    {
                        Name = "Lead Walker",
                        Address = _emailConfiguration.SmtpUsername
                    },
                    Subject = "You received a Inquiry!",
                    Content = await _templateHelper.GetTemplateHtmlAsStringAsync("Templates/ContactMessage", contactMessage)
                };

                _emailService.Send(emailMessage);

                return RedirectToAction("Index").WithSuccess("Your message has been sent.", null); ;
            }

            var item = _context.Contacts.FirstOrDefault();
            var footer = _context.BusinessProfile.FirstOrDefault();

            ContactViewModel model = new ContactViewModel()
            {
                _contact = item,
                BusinessProfile = footer


            };
            return View(model).WithWarning("Please Try again.", null);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cart)
        {
            cart.Id = 0;
            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _context.ShoppingCarts.Where(c => c.ApplicationUserId == cart.ApplicationUserId && c.ItemId == cart.ItemId).FirstOrDefaultAsync();
                if(cartFromDb == null)
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
       
    }

}


    

