﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasePackageModule2.Data;
using BasePackageModule2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule2.ViewComponents
{
    [ViewComponent(Name = "Footer")]
    public class Footer : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public Footer(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var businessProfile = new Models.BusinessProfile();
            if (_context.BusinessProfile.Any())
            {
                businessProfile = await _context.BusinessProfile.FirstOrDefaultAsync();
            }

            NavViewModel model = new NavViewModel
            {

                BusinessProfile = businessProfile,


            };

            return View("Index", model);
        }
    }
}