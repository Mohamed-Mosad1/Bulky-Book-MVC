// Ignore Spelling: BLL

using BulkyBook.Model.OrdersAggregate;

namespace BulkyBook.BLL.Services.Contract
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string userId, string cartId, OrderAddress orderAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string userId);
        Task<Order?> GetOrderByIdAsync(string orderId);

    }
}
