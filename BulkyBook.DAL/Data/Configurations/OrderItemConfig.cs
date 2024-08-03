using BulkyBook.Model.OrdersAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BulkyBook.DAL.Data.Configurations
{
    public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(orderItem => orderItem.ProductOrdered, product => product.WithOwner());
            builder.Property(orderItem => orderItem.Price).HasColumnType("decimal(18, 2)");
        }
    }
}
