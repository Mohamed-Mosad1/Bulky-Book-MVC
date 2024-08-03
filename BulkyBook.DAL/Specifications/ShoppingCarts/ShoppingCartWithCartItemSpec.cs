using BulkyBook.Model.Cart;

namespace BulkyBook.DAL.Specifications.ShoppingCarts
{
    public class ShoppingCartWithCartItemSpec : BaseSpecifications<ShoppingCart>
    {
        public ShoppingCartWithCartItemSpec(string userId)
        : base(x => x.AppUserId == userId)
        {
            AddInclude(x => x.CartItems);
            AddInclude("CartItems.Product");
            AddInclude("CartItems.Product.ProductImages");
        }
    }
}
