// Ignore Spelling: App

using BulkyBook.Model.OrdersAggregate;

namespace BulkyBook.Model.ViewModels
{
    public class ShoppingCartVM
    {
        public string Id { get; set; } = null!;
        public List<ShoppingCartItemVM> CartItems { get; set; } = new List<ShoppingCartItemVM>();
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public string AppUserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string State { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public decimal TotalPrice { get; set; }
    }
}
