// Ignore Spelling: BLL

using BulkyBook.Model.OrdersAggregate;
using Stripe.Checkout;

namespace BulkyBook.BLL.Services.Contract
{
    public interface IPaymentService
    {
        Task<Session?> CreateSessionPaymentAsync(string orderId);
        Task UpdatePaymentIntentIdAndSessionIdAsync(Order order, string sessionId, string? paymentIntentId);
        Task UpdateOrderAndPaymentStatusAsync(string orderId, OrderStatus orderStatus, PaymentStatus? paymentStatus);
    }
}
