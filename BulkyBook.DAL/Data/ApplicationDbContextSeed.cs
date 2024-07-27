// Ignore Spelling: Admin

using BulkyBook.Model;
using BulkyBook.Model.Identity;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                  new Product { Id = 1, Title = "Fortune of Time", Author = "Billy Spark", Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.", ISBN = "SWD9999001", ListPrice = 99, Price = 90, Price50 = 85, Price100 = 80, CategoryId = 1 },
                  new Product { Id = 2, Title = "Dark Skies", Author = "Nancy Hoover", Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.", ISBN = "CAW777777701", ListPrice = 40, Price = 30, Price50 = 25, Price100 = 20, CategoryId = 1 },
                  new Product { Id = 3, Title = "Vanish in the Sunset", Author = "Julian Button", Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.", ISBN = "RITO5555501", ListPrice = 55, Price = 50, Price50 = 40, Price100 = 35, CategoryId = 1 },
                  new Product { Id = 4, Title = "Cotton Candy", Author = "Abby Muscles", Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.", ISBN = "WS3333333301", ListPrice = 70, Price = 65, Price50 = 60, Price100 = 55, CategoryId = 2 },
                  new Product { Id = 5, Title = "Rock in the Ocean", Author = "Ron Parker", Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.", ISBN = "SOTJ1111111101", ListPrice = 30, Price = 27, Price50 = 25, Price100 = 20, CategoryId = 2 },
                  new Product { Id = 6, Title = "Leaves and Wonders", Author = "Laura Phantom", Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.", ISBN = "FOT000000001", ListPrice = 25, Price = 23, Price50 = 22, Price100 = 20, CategoryId = 3 }
                );

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
