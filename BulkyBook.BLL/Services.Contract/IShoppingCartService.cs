using BulkyBook.Model.Cart;

namespace BulkyBook.DAL.InterFaces
{
    public interface IShoppingCartService
    {
        Task AddOrUpdateToCartAsync(string userId, int productId, int quantity);
        Task<ShoppingCart> GetOrCreateCartAsync(string userId);
        Task<ShoppingCartItem?> GetCartItemByIdAsync(string cartItemId);
        Task<IEnumerable<ShoppingCart>> GetAllCartsAsync(string userId);
        Task RemoveCartAsync(string cartId);
        Task RemoveCartItemAsync(string cartItemId);
        decimal GetPriceBasedOnQuantity(ShoppingCartItem shoppingCartItem);
        Task IncrementCartItemAsync(string cartItemId);
        Task DecrementCartItemAsync(string cartItemId);
    }
}
