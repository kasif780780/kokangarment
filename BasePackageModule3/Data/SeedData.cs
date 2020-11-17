using System;
using System.Linq;
using BasePackageModule3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BasePackageModule3.Data
{
    public static class SeedData
    {
        public static async void InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // await context.Database.EnsureDeletedAsync();

            await context.Database.EnsureCreatedAsync();

            if (!context.Users.Any())
            {

                ApplicationUser user = new ApplicationUser
                {
                    Email = "admin@admin.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "admin@admin.com",
                };


                var adminUser = await userManager.CreateAsync(user, "Admin@123");

                if (adminUser.Succeeded)
                {

                    await roleManager.CreateAsync(new IdentityRole
                    {
                        Name = "Admin"
                    });
                    await roleManager.CreateAsync(new IdentityRole
                    {
                        Name = "User"
                    });

                    await userManager.AddToRoleAsync(user, "Admin");
                }
                await context.SaveChangesAsync();
            }
        }

    }
}
