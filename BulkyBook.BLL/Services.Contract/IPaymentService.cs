// Ignore Spelling: BLL

using BulkyBook.Model.Cart;
using BulkyBook.Model.OrdersAggregate;
using Stripe.Checkout;

namespace BulkyBook.BLL.Services.Contract
{
    public interface IPaymentService
    {
        Task<Session?> CreateSessionPaymentAsync(ShoppingCart shoppingCart, Order order);
        Task<Session?> CreateSessionPaymentForCompanyAsync(string orderId);
        void UpdatePaymentIntentIdAndSessionIdAsync(Order order, string sessionId, string? paymentIntentId);
        Task UpdateOrderAndPaymentStatusAsync(string orderId, OrderStatus orderStatus, PaymentStatus? paymentStatus);
    }
}
