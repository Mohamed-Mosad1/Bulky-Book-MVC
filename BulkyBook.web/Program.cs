using BulkyBook.BLL.Services;
using BulkyBook.BLL.Services.Contract;
using BulkyBook.DAL.Data;
using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Repositories;
using BulkyBook.Model.Identity;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                // Sign-In settings
                options.SignIn.RequireConfirmedEmail = true; // Requires email confirmation before sign-in
                options.User.RequireUniqueEmail = true; // Ensures unique emails across users

                // Password settings
                options.Password.RequireDigit = true; // Requires at least one numeric character
                options.Password.RequiredLength = 6; // Minimum password length
                options.Password.RequireNonAlphanumeric = false; // No need for special characters
                options.Password.RequireUppercase = false; // No need for uppercase letters
                options.Password.RequireLowercase = false; // No need for lowercase letters

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Account is locked for 5 minutes after max failed attempts
                options.Lockout.MaxFailedAccessAttempts = 5; // Maximum failed access attempts before account is locked
                options.Lockout.AllowedForNewUsers = true; // Lockout enabled for new users
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
                options.SlidingExpiration = true;
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            builder.Services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = builder.Configuration.GetValue<string>("FacebookSettings:AppId") ?? string.Empty;
                options.AppSecret = builder.Configuration.GetValue<string>("FacebookSettings:AppSecret") ?? string.Empty;
            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(100);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddScoped(typeof(IEmailSender), typeof(EmailSender));

            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            builder.Services.AddScoped(typeof(IProductService), typeof(ProductService));
            builder.Services.AddScoped(typeof(IShoppingCartService), typeof(ShoppingCartService));
            builder.Services.AddScoped(typeof(IOrderService), typeof(OrderService));
            builder.Services.AddScoped(typeof(IPaymentService), typeof(PaymentService));

            builder.Services.AddAutoMapper(typeof(MappingProfiles));


            var app = builder.Build();

            #region Apple All Pending Migrations and Data Seeding

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var logger = loggerFactory.CreateLogger<Program>();

                try
                {
                    await dbContext.Database.MigrateAsync();
                    await ApplicationDbContextSeed.SeedRolesAsync(roleManager);
                    await ApplicationDbContextSeed.SeedAdminAsync(userManager);
                    await ApplicationDbContextSeed.SeedCategoriesAsync(dbContext);
                    await ApplicationDbContextSeed.SeedProductsAsync(dbContext);
                    await ApplicationDbContextSeed.SeedCompaniesAsync(dbContext);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while updating the database.");
                }
            }

            #endregion

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
