using BulkyBook.Model.Cart;

namespace BulkyBook.DAL.Specifications.ShoppingCarts
{
    public class ShoppingCartWithCartItemSpec : BaseSpecifications<ShoppingCart>
    {
        public ShoppingCartWithCartItemSpec(string id)
        : base(x => x.AppUserId == id)
        {
            Includes.Add(x => x.CartItems);
        }
    }
}
