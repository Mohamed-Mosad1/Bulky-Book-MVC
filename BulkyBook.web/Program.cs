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
                //options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            //builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
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

            //Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetConnectionString("Stripe:SecretKey");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
