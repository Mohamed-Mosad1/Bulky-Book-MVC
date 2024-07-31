// Ignore Spelling: App

namespace BulkyBook.Model.ViewModels
{
    public class ShoppingCartVM
    {
        public string Id { get; set; } = null!;
        public List<ShoppingCartItemVM> CartItems { get; set; } = new List<ShoppingCartItemVM>();
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public string AppUserId { get; set; } = null!;

    }
}
