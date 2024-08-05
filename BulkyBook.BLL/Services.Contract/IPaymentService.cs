// Ignore Spelling: BLL

using BulkyBook.Model.Cart;
using BulkyBook.Model.OrdersAggregate;

namespace BulkyBook.BLL.Services.Contract
{
    public interface IPaymentService
    {
        Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId);

        Task<Order> UpdatePaymentIntentToSucceedOrFailed(string paymentIntentId, bool flag);
    }
}
