using BulkyBook.Model.Cart;

namespace BulkyBook.DAL.InterFaces
{
    public interface IShoppingCartService
    {
        Task AddOrUpdateToCartAsync(string userId, int productId, int quantity);
        Task<ShoppingCart?> GetCartAsync(string userId, bool includeCartItem = false, bool includeImages = false);
        Task<ShoppingCartItem?> GetCartItemByIdAsync(string cartItemId);
        Task RemoveCartAsync(string cartId);
        Task RemoveCartItemAsync(string cartItemId);
        decimal GetPriceBasedOnQuantity(ShoppingCartItem shoppingCartItem);
        Task IncrementCartItemAsync(string cartItemId);
        Task DecrementCartItemAsync(string cartItemId);
        Task<int> GetCountAsync(string userId);
    }
}
