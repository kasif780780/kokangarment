using BasePackageModule3.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BasePackageModule3.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<FindJob> FindJobs { get; set; }
        public DbSet<Footer> Footers { get; set; }
        public DbSet<Update> Updates { get; set; }
        public DbSet<NewProject> NewProjects {get; set; }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<BannerBottom> BannerBottoms { get; set; }
        public DbSet<SliderImage> SliderImages { get; set; }
        public DbSet<TopBar> TopBars { get; set; }
        public DbSet<Banner> Banners { get; set; }
        
        public DbSet<Page> Pages { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Logo> Logos { get; set; }
        public DbSet<Seo> Seos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }







    }
}
