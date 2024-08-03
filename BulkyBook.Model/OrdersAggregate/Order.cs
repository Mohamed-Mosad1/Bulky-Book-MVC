// Ignore Spelling: App

using BulkyBook.Model.Identity;

namespace BulkyBook.Model.OrdersAggregate
{
    public class Order
    {
        private Order() { }

        public Order(string id, string appUserId, AppUser appUser, DateTimeOffset orderDate, DateTimeOffset shippingDate, OrderStatus orderStatus, OrderAddress orderAddress, ICollection<OrderItem> orderItems, decimal orderTotal, string paymentInstantId, PaymentStatus paymentStatus, string? trackingNumber, string? carrier, DateTimeOffset paymentDate, DateOnly paymentDueDate)
        {
            Id = id;
            AppUserId = appUserId;
            AppUser = appUser;
            OrderDate = orderDate;
            ShippingDate = shippingDate;
            OrderStatus = orderStatus;
            OrderAddress = orderAddress;
            OrderItems = orderItems;
            OrderTotal = orderTotal;
            PaymentInstantId = paymentInstantId;
            PaymentStatus = paymentStatus;
            TrackingNumber = trackingNumber;
            Carrier = carrier;
            PaymentDate = paymentDate;
            PaymentDueDate = paymentDueDate;
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset ShippingDate { get; set; } = DateTimeOffset.UtcNow;
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public OrderAddress OrderAddress { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public decimal OrderTotal { get; set; }

        public string PaymentInstantId { get; set; } = null!;
        public PaymentStatus PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTimeOffset PaymentDate { get; set; } = DateTimeOffset.UtcNow;
        public DateOnly PaymentDueDate { get; set; }

    }
}
