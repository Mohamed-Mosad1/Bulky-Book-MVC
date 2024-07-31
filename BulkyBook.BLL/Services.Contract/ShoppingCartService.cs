using AutoMapper;
using BulkyBook.DAL.Data;
using BulkyBook.DAL.InterFaces;
using BulkyBook.Model.Cart;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DAL.Repositories
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ShoppingCartService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task AddOrUpdateToCartAsync(string userId, int productId, int quantity)
        {
            var shoppingCart = await GetOrCreateCartAsync(userId);
            var cartItem = shoppingCart.CartItems.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem is not null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                var newCartItem = new ShoppingCartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                };

                shoppingCart.CartItems.Add(newCartItem);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<ShoppingCart> GetOrCreateCartAsync(string userId)
        {
            var shoppingCart = await _dbContext.ShoppingCarts
                              .Include(c => c.CartItems)
                              .ThenInclude(c => c.Product)
                              .ThenInclude(c => c.ProductImages)
                              .FirstOrDefaultAsync(c => c.AppUserId == userId);

            if (shoppingCart is null)
            {
                shoppingCart = new ShoppingCart
                {
                    AppUserId = userId
                };

                _dbContext.ShoppingCarts.Add(shoppingCart);
                await _dbContext.SaveChangesAsync();
            }


            return shoppingCart;
        }

        public async Task<ShoppingCartItem?> GetCartItemByIdAsync(string cartItemId)
        {
            return await _dbContext.ShoppingCartItems.FirstOrDefaultAsync(c => c.Id == cartItemId);
        }

        public async Task<IEnumerable<ShoppingCart>> GetAllCartsAsync(string userId)
        {
            return await _dbContext.ShoppingCarts.Where(c => c.AppUserId == userId).ToListAsync();
        }

        public async Task RemoveCartItemAsync(string cartItemId)
        {
            var shoppingCartItem = await _dbContext.ShoppingCartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId);


            if (shoppingCartItem is not null)
            {
                _dbContext.ShoppingCartItems.Remove(shoppingCartItem);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveCartAsync(string cartId)
        {
            var shoppingCart = await _dbContext.ShoppingCarts.FirstOrDefaultAsync(x => x.Id == cartId);

            if (shoppingCart is not null)
            {
                _dbContext.ShoppingCarts.Remove(shoppingCart);

                await _dbContext.SaveChangesAsync();
            }
        }


        public decimal GetPriceBasedOnQuantity(ShoppingCartItem shoppingCartItem)
        {
            return shoppingCartItem.Quantity switch
            {
                <= 50 => shoppingCartItem.Product.Price,
                <= 100 => shoppingCartItem.Product.Price50,
                _ => shoppingCartItem.Product.Price100
            };

        }

        public async Task IncrementCartItemAsync(string cartItemId)
        {
            var cartItem = await _dbContext.ShoppingCartItems.FirstOrDefaultAsync(u => u.Id == cartItemId);
            if (cartItem is not null)
            {
                cartItem.Quantity += 1;
                _dbContext.ShoppingCartItems.Update(cartItem);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DecrementCartItemAsync(string cartItemId)
        {
            var cartItem = await _dbContext.ShoppingCartItems.FirstOrDefaultAsync(u => u.Id == cartItemId);
            if (cartItem is not null)
            {
                if (cartItem.Quantity <= 1)
                {
                    _dbContext.ShoppingCartItems.Remove(cartItem);

                    var cart = await _dbContext.ShoppingCarts
                        .Include(c => c.CartItems)
                        .FirstOrDefaultAsync(c => c.Id == cartItem.Id);

                    if (cart is not null && !cart.CartItems.Any())
                    {
                        _dbContext.ShoppingCarts.Remove(cart);
                    }
                }
                else
                {
                    cartItem.Quantity -= 1;
                    _dbContext.ShoppingCartItems.Update(cartItem);
                }

                await _dbContext.SaveChangesAsync();
            }
        }


    }
}
