using BulkyBook.DAL.Data;
using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Repositories;
using BulkyBook.web.Helpers;
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

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            builder.Services.AddAutoMapper(typeof(MappingProfiles));


            var app = builder.Build();

            #region Apple All Pending Migrations

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Program>();

                try
                {
                    await dbContext.Database.MigrateAsync();
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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
