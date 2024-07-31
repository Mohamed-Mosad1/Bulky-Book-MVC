using BulkyBook.Model.Cart;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BulkyBook.DAL.Data.Configurations
{
    public class ShoppingCartItemConfig : IEntityTypeConfiguration<ShoppingCartItem>
    {
        public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
        {
            // One-to-Many relationship with Product
            builder.HasOne(x => x.Product)
                   .WithMany()
                   .HasForeignKey(x => x.ProductId)
                   .OnDelete(DeleteBehavior.NoAction);

            // One-to-Many relationship with AppUser
            //builder.HasOne(x => x.AppUser)
            //       .WithMany()
            //       .HasForeignKey(x => x.AppUserId)
            //       .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
