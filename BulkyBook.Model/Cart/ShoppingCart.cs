// Ignore Spelling: App

using BulkyBook.Model.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Model.Cart
{
    public class ShoppingCart
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }

        public ICollection<ShoppingCartItem> CartItems { get; set; } = new HashSet<ShoppingCartItem>();

        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;

        [NotMapped]
        public decimal TotalPrice { get; set; }
    }
}
