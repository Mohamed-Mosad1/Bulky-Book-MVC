// Ignore Spelling: App

using BulkyBook.Model.Identity;

namespace BulkyBook.Model.OrdersAggregate
{
    public class Order
    {
        public Order() { }

        public Order(string appUserId, OrderStatus orderStatus, OrderAddress orderAddress, ICollection<OrderItem> orderItems, decimal orderTotal, string paymentInstantId, PaymentStatus paymentStatus)
        {
            AppUserId = appUserId;
            OrderStatus = orderStatus;
            OrderAddress = orderAddress;
            OrderItems = orderItems;
            OrderTotal = orderTotal;
            PaymentInstantId = paymentInstantId;
            PaymentStatus = paymentStatus;
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ShippingDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public OrderAddress OrderAddress { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public decimal OrderTotal { get; set; }

        public string? PaymentInstantId { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTimeOffset? PaymentDate { get; set; }
        public DateOnly? PaymentDueDate { get; set; }

    }
}
