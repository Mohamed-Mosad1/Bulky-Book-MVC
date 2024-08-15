using BulkyBook.Model.Cart;

namespace BulkyBook.DAL.Specifications.ShoppingCarts
{
    public class ShoppingCartWithCartItemSpec : BaseSpecifications<ShoppingCart>
    {
        public ShoppingCartWithCartItemSpec(string userId, bool includeCartItem = false, bool includeProducts = false, bool includeImages = false)
        : base(x => x.AppUserId == userId)
        {
            AddInclude(x => x.AppUser);

            if (includeCartItem)
            {
                AddInclude(x => x.CartItems);

                if (includeProducts)
                {
                    AddInclude("CartItems.Product");

                    if (includeImages)
                    {
                        AddInclude("CartItems.Product.ProductImages");
                    }
                }
            }

            ///if (includeCartItem)
            ///{
            ///    AddSelect(x => new ShoppingCart()
            ///    {
            ///        AppUser = new AppUser()
            ///        {
            ///            Id = x.AppUser.Id,
            ///            UserName = x.AppUser.UserName,
            ///            Email = x.AppUser.Email,
            ///            CompanyId = x.AppUser.CompanyId,
            ///        },
            ///        CartItems = includeCartItem ? x.CartItems.Select(ci => new ShoppingCartItem()
            ///        {
            ///            Id = ci.Id,
            ///            ProductId = ci.ProductId,
            ///            Quantity = ci.Quantity,
            ///            ShoppingCartId = ci.ShoppingCartId,
            ///            Product = includeProducts ? new Product()
            ///            {
            ///                Id = ci.Product.Id,
            ///                Title = ci.Product.Title,
            ///                Price = ci.Product.Price,
            ///                Description = ci.Product.Description,
            ///                ProductImages = includeImages ? ci.Product.ProductImages.Select(pi => new ProductImage()
            ///                {
            ///                    ImageUrl = pi.ImageUrl
            ///                }).ToList() : new List<ProductImage>()
            ///            } : null!,
            ///        }).ToList() : new List<ShoppingCartItem>()
            ///    });
            ///}

        }


    }
}
