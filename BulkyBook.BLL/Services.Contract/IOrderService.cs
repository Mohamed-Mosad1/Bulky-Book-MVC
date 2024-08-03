// Ignore Spelling: BLL

using BulkyBook.Model.OrdersAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.BLL.Services.Contract
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string userId, string cartId, OrderAddress orderAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string userId);
        Task<Order> GetOrderByIdAsync(string orderId, string userId);

    }
}
