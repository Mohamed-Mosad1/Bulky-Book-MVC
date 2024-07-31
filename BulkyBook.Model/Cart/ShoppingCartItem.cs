// Ignore Spelling: App

namespace BulkyBook.Model.Cart
{
    public class ShoppingCartItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int Quantity { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string ShoppingCartId { get; set; } = null!;
        public ShoppingCart ShoppingCart { get; set; } = null!;


    }
}
