using BulkyBook.Model.Cart;

namespace BulkyBook.DAL.Specifications.ShoppingCarts
{
    public class ShoppingCartItemByProductSpec : BaseSpecifications<ShoppingCartItem>
    {
        public ShoppingCartItemByProductSpec(int productId)
            : base(x => x.ProductId == productId)
        {

        }
    }
}
