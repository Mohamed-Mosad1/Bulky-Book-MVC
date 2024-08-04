using BulkyBook.Model.Cart;

namespace BulkyBook.DAL.Specifications.ShoppingCarts
{
    public class ShoppingCartWithSpec : BaseSpecifications<ShoppingCart>
    {
        public ShoppingCartWithSpec(string userId)
            : base(x => x.AppUserId == userId)
        {

        }
    }
}
