using BulkyBook.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BulkyBook.DAL.Data.Configurations
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.ListPrice).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Price50).HasColumnType("decimal(18,2)");
            builder.Property(p => p.Price100).HasColumnType("decimal(18,2)");

        }
    }
}
