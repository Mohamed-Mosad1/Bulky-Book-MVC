// Ignore Spelling: App

using BulkyBook.Model.Identity;

namespace BulkyBook.Model
{
    public class ShoppingCart : BaseModel
    {
        public int Quantity { get; set; }

        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
