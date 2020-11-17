using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BasePackageModule3.Data;
using BasePackageModule3.Extensions;
using BasePackageModule3.Helpers;
using BasePackageModule3.Models;
using BasePackageModule3.Utility;
using BasePackageModule3.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule3.Controllers
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

        public async Task<IActionResult> IndexAsync()
        {
            AboutUs about = await _context.AboutUs.AnyAsync() ? await _context.AboutUs.FirstOrDefaultAsync() : new AboutUs();
           
            var banner = await _context.Banners.FirstOrDefaultAsync();
            
            List<SliderImage> images = await _context.SliderImages.ToListAsync();
            var newProject = await _context.NewProjects.ToListAsync();
            var mycorouses = await _context.Courses.OrderBy(o=>o.Order).Take(4).ToListAsync();
            var sscourse = await _context.Courses.FirstOrDefaultAsync();
            var update = await _context.Posts.OrderByDescending(s => s.Id).Take(4).ToListAsync();
            var gallery = await _context.Images.OrderByDescending(s => s.Id).Take(8).ToListAsync();
            var services = await _context.Services.OrderByDescending(s => s.Id).Take(4).ToListAsync();
            var ssservice = await _context.Services.FirstOrDefaultAsync();
            var snewproject = await _context.NewProjects.FirstOrDefaultAsync();
            var supdate = await _context.Posts.FirstOrDefaultAsync();
            var footer = await _context.Footers.FirstOrDefaultAsync();
            var item = await _context.Items.OrderByDescending(s => s.Id).Take(4).ToListAsync();
            var sitem = await _context.Items.FirstOrDefaultAsync();
                 

            HomeViewModel model = new HomeViewModel
            {
                SliderImages = images,
                _AboutUs = about,
                _NewProject =newProject,
                _Update = update,
                _Image = gallery,
                _service = services,
                _snewproject = snewproject,
                _supdate =supdate,
                _footer = footer,
                _ssservice = ssservice,
                _item = item,
                _sitem = sitem,
                _corses = mycorouses,
                _sscourse = sscourse

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

        // POST: Admin/FindJobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetJob([Bind("FindJobId,FullName,Mobile,Email,City,Location,Age,Gender,Qualification,SchoolMedium,SpeakEnglish,Experience,JobRole,Resume")] FindJob findJob)
        {
            if (ModelState.IsValid)
            {
                _context.Add(findJob);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Thankyou));
            }
            return View(findJob);
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
        public async Task<IActionResult> Service(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var footer = await _context.Services
                .FirstOrDefaultAsync(m => m.Id == id);
            if (footer == null)
            {
                return NotFound();
            }

            return View(footer);
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var footer = await _context.Updates
                .FirstOrDefaultAsync(m => m.Id == id);
            if (footer == null)
            {
                return NotFound();
            }

            return View(footer);
        }
        public async Task<IActionResult> NewProject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var footer = await _context.NewProjects
                .FirstOrDefaultAsync(m => m.NewProjectId == id);
            if (footer == null)
            {
                return NotFound();
            }

            return View(footer);
        }

        public IActionResult Contact()
        {
            var footer = new Models.Footer();


          
            if (_context.Footers.Any())
            {
                footer = _context.Footers.FirstOrDefault();
            }
            var item = _context.Contacts.FirstOrDefault();
            ContactViewModel model = new ContactViewModel()
            {
                _contact = item,
                _footer = footer

                
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
            var footer = _context.Footers.FirstOrDefault();

            ContactViewModel model = new ContactViewModel()
            {
                _contact = item,
                _footer = footer


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
                var itemfromdb = await _context.Items.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == cart.ItemId).FirstOrDefaultAsync();
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


    

