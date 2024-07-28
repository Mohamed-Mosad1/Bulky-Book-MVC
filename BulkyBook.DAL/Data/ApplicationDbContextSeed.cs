// Ignore Spelling: Admin

using BulkyBook.Model;
using BulkyBook.Model.Identity;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace BulkyBook.DAL.Data
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                await roleManager.CreateAsync(new IdentityRole(SD.Role_Company));
            }
        }

        public static async Task SeedAdminAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any(x => x.UserName == SD.Role_Admin))
            {
                var user = new AppUser
                {
                    UserName = SD.Role_Admin,
                    Email = SD.Admin_Email,
                    City = "Alexandria",
                    Country = "Egypt",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,

                };

                await userManager.CreateAsync(user, SD.Admin_Password);
                await userManager.AddToRoleAsync(user, SD.Role_Admin);
            }
        }

        public static async Task SeedProductsAsync(ApplicationDbContext dbContext)
        {
            if (!dbContext.Products.Any())
            {
                await dbContext.Set<Product>().AddRangeAsync(
                  new Product { Title = "Fortune of Time", Author = "Billy Spark", Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.", ISBN = "SWD9999001", ListPrice = 99, Price = 90, Price50 = 85, Price100 = 80, CategoryId = 1 },
                  new Product { Title = "Dark Skies", Author = "Nancy Hoover", Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.", ISBN = "CAW777777701", ListPrice = 40, Price = 30, Price50 = 25, Price100 = 20, CategoryId = 1 },
                  new Product { Title = "Vanish in the Sunset", Author = "Julian Button", Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.", ISBN = "RITO5555501", ListPrice = 55, Price = 50, Price50 = 40, Price100 = 35, CategoryId = 1 },
                  new Product { Title = "Cotton Candy", Author = "Abby Muscles", Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.", ISBN = "WS3333333301", ListPrice = 70, Price = 65, Price50 = 60, Price100 = 55, CategoryId = 2 },
                  new Product { Title = "Rock in the Ocean", Author = "Ron Parker", Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.", ISBN = "SOTJ1111111101", ListPrice = 30, Price = 27, Price50 = 25, Price100 = 20, CategoryId = 2 },
                  new Product { Title = "Leaves and Wonders", Author = "Laura Phantom", Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.", ISBN = "FOT000000001", ListPrice = 25, Price = 23, Price50 = 22, Price100 = 20, CategoryId = 3 }
                );

                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedCompaniesAsync(ApplicationDbContext dbContext)
        {
            if (!dbContext.Companies.Any())
            {
                await dbContext.Set<Company>().AddRangeAsync(
                    new Company { Name = "Tech Solution", Street = "123 Tech St", Country = "Egypt", City = "Tech City", State = "IL", PhoneNumber = "6669990000" },
                    new Company { Name = "Vivid Books", Street = "999 Vid St", Country = "UAE", City = "Vid City", State = "IL", PhoneNumber = "7779990000" },
                    new Company { Name = "Readers Club", Street = "999 Main St", Country = "Saudi Arabia", City = "Lala land", State = "NY", PhoneNumber = "1113335555" }
                );

                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedCategoriesAsync(ApplicationDbContext dbContext)
        {
            if (!dbContext.Categories.Any())
            {
                await dbContext.Set<Category>().AddRangeAsync(
                    new Category { Name = "Action", DisplayOrder = 1 },
                    new Category { Name = "SciFi", DisplayOrder = 2 },
                    new Category { Name = "History", DisplayOrder = 3 }
                );

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
