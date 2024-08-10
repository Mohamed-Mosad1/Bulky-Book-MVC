// Ignore Spelling: BLL

using BulkyBook.Model.Cart;
using BulkyBook.Model.OrdersAggregate;

namespace BulkyBook.BLL.Services.Contract
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string userId, ShoppingCart shoppingCart, OrderAddress orderAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string userId);
        Task<Order?> GetOrderByIdAsync(string orderId, bool includeUser = false, bool includeOrderItems = false);

    }
}
