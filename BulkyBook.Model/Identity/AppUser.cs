// Ignore Spelling: App

using BulkyBook.Model.Cart;
using BulkyBook.Model.OrdersAggregate;
using Microsoft.AspNetCore.Identity;

namespace BulkyBook.Model.Identity
{
    public class AppUser : IdentityUser
    {
        public string? Street { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string Country { get; set; } = null!;

        public int? CompanyId { get; set; }
        public Company? Company { get; set; }

        public ICollection<ShoppingCart> ShoppingCarts { get; set; } = new HashSet<ShoppingCart>();
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
