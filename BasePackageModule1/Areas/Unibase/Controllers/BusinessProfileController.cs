using System.Linq;
using System.Threading.Tasks;
using BasePackageModule1.Data;
using BasePackageModule1.Extensions;
using BasePackageModule1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule1.Areas.Unibase.Controllers
{
    [Area("Unibase")]
    public class BusinessProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BusinessProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var businessProfile = await _context.BusinessProfile.FirstOrDefaultAsync();
            return businessProfile == null ? View(new BusinessProfile()) : View(businessProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BusinessProfile businessProfile)
        {
            
            if (ModelState.IsValid)
            {
                if (await _context.BusinessProfile.AnyAsync())
                {
                    try
                    {
                        _context.BusinessProfile.Update(businessProfile);

                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!BusinessProfileExists(businessProfile.Id))
                        {
                            return NotFound();
                        }

                        throw;
                    }
                    return RedirectToAction(nameof(Index)).WithSuccess("Business Profile has been Updated.", null);
                }

                _context.Add(businessProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)).WithSuccess("Business Profile has been Updated.", null);
            }

            return View(businessProfile).WithError("Please fill all required details.", null);
        }

        private bool BusinessProfileExists(int id)
        {
            return _context.BusinessProfile.Any(e => e.Id == id);
        }
    }
}
