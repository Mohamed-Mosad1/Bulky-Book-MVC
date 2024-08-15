// Ignore Spelling: App

namespace BulkyBook.Model.ViewModels.OrderVM
{
    public class OrderToReturnVM
    {
        public string Id { get; set; } = null!;
        public string AppUserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public DateTimeOffset OrderDate { get; set; }
        public DateTimeOffset? ShippingDate { get; set; }
        public string? OrderStatus { get; set; }
        public OrderAddressVM OrderAddress { get; set; } = null!;

        public ICollection<OrderItemsVM> OrderItems { get; set; } = new HashSet<OrderItemsVM>();
        public decimal OrderTotal { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTimeOffset? PaymentDate { get; set; }
        public DateOnly? PaymentDueDate { get; set; }
    }
}
