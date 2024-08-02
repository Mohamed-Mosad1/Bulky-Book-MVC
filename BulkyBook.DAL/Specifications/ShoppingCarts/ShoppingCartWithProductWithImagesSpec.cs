using BulkyBook.Model.Cart;

namespace BulkyBook.DAL.Specifications.ShoppingCarts
{
    public class ShoppingCartWithProductWithImagesSpec : BaseSpecifications<ShoppingCartItem>
    {
        public ShoppingCartWithProductWithImagesSpec(string cartId)
                : base(x => x.Id == cartId)
        {
            Includes.Add(x => x.Product);
			Includes.Add(x => x.Product.ProductImages);
        }
    }
}
