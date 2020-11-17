using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BasePackageModule3.Data;
using BasePackageModule3.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace BasePackageModule3.ViewComponents
{
    [ViewComponent(Name = "ChatBot")]
    public class ChatBot: ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public ChatBot(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var snewproject = new Models.Footer();
            if (await _context.Footers.AnyAsync())
            {
                 snewproject = await _context.Footers.FirstOrDefaultAsync();
            }
           

            NavViewModel model = new NavViewModel
            {



                _footer = snewproject,


            };

            return View("Index", model);
        }
    }
}
