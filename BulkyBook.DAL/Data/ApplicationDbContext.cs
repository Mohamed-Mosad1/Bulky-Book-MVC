using BulkyBook.Model;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BulkyBook.DAL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Category>()
                        .HasData(
                            new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                            new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                            new Category { Id = 3, Name = "History", DisplayOrder = 3 }
                        );
        }

        public DbSet<Category> Categories { get; set; }
    }
}
