using BulkyBook.Model.OrdersAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BulkyBook.DAL.Data.Configurations
{
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(order => order.OrderAddress, orderAddress => orderAddress.WithOwner());
            builder.Property(order => order.OrderStatus).HasConversion(orderStatus => orderStatus.ToString(), orderStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), orderStatus));
            builder.Property(order => order.PaymentStatus).HasConversion(paymentStatus => paymentStatus.ToString(), paymentStatus => (PaymentStatus)Enum.Parse(typeof(PaymentStatus), paymentStatus));
            builder.Property(order => order.OrderTotal).HasColumnType("decimal(18, 2)");
            builder.HasMany(order => order.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
