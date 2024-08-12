using BulkyBook.Model;
using BulkyBook.Model.Cart;
using BulkyBook.Model.Identity;

namespace BulkyBook.DAL.Specifications.ShoppingCarts
{
    public class ShoppingCartWithCartItemSpec : BaseSpecifications<ShoppingCart>
    {
        public ShoppingCartWithCartItemSpec(string userId, bool includeCartItem = false, bool includeImages = false)
        : base(x => x.AppUserId == userId)
        {
            ApplyNoTracking();

            if (includeCartItem)
            {
                AddSelect(x => new ShoppingCart()
                {
                    AppUser = new AppUser()
                    {
                        Id = x.AppUser.Id,
                        UserName = x.AppUser.UserName,
                        Email = x.AppUser.Email,
                        CompanyId = x.AppUser.CompanyId,
                    },
                    CartItems = includeCartItem ? x.CartItems.Select(ci => new ShoppingCartItem()
                    {
                        Id = ci.Id,
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        Product = new Product()
                        {
                            Id = ci.Product.Id,
                            Title = ci.Product.Title,
                            Price = ci.Product.Price,
                            Description = ci.Product.Description,
                            ProductImages = includeImages ? ci.Product.ProductImages.Select(pi => new ProductImage()
                            {
                                ImageUrl = pi.ImageUrl
                            }).ToList() : new List<ProductImage>()
                        }
                    }).ToList() : new List<ShoppingCartItem>()
                });
            }

        }
    }
}
