// Ignore Spelling: BLL

using BulkyBook.Model.Cart;
using BulkyBook.Model.OrdersAggregate;
using BulkyBook.Model.ViewModels.OrderVM;

namespace BulkyBook.BLL.Services.Contract
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string userId, ShoppingCart shoppingCart, OrderAddress orderAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string? userOrOrderId = null, bool includeUser = false, bool includeOrderItems = false);
        Task<Order?> GetOrderByIdAsync(string userId, bool includeUser = false, bool includeOrderItems = false);
        Task<bool> UpdateOrderDetailsAsync(OrderToReturnVM orderToReturnVM);
        Task<bool> UpdateOrderToShippedAsync(OrderToReturnVM orderToReturnVM);
        Task<bool> UpdateOrderToCancelAsync(OrderToReturnVM orderToReturnVM);
    }
}
