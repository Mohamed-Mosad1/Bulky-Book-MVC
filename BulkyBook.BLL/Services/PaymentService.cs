// Ignore Spelling: BLL

using BulkyBook.BLL.Services.Contract;
using BulkyBook.DAL.InterFaces;
using BulkyBook.Model.Cart;
using BulkyBook.Model.OrdersAggregate;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace BulkyBook.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            IConfiguration configuration,
            IShoppingCartService shoppingCartService,
            IUnitOfWork unitOfWork
            )
        {
            _configuration = configuration;
            _shoppingCartService = shoppingCartService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Session?> CreateSessionPaymentAsync(ShoppingCart shoppingCart, Order order)
        {
            // Get Secret key
            StripeConfiguration.ApiKey = _configuration["Stripe:Secretkey"];

            if (shoppingCart is null)
                return null;

            var domain = _configuration["BaseUrl"];
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?orderId={order.Id}",
                CancelUrl = domain + "customer/cart/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            if (shoppingCart.CartItems.Count > 0)
            {
                foreach (var item in shoppingCart.CartItems)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.Product.Price * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Quantity
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                await _unitOfWork.CompleteAsync();
            }

            var sessionService = new SessionService();
            var session = await sessionService.CreateAsync(options);

            UpdatePaymentIntentIdAndSessionIdAsync(order, session.Id, session.PaymentIntentId);

            shoppingCart.LastSessionId = session.Id;
            //_unitOfWork.Repository<ShoppingCart>().Update(shoppingCart);

            await _unitOfWork.CompleteAsync();

            return session;
        }


        public void UpdatePaymentIntentIdAndSessionIdAsync(Order order, string sessionId, string? paymentIntentId)
        {
            if (order is null)
                return;

            if (!string.IsNullOrEmpty(sessionId))
                order.SessionId = sessionId;

            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                order.PaymentIntentId = paymentIntentId;
                order.PaymentDate = DateTimeOffset.UtcNow;
            }
        }

        public async Task UpdateOrderAndPaymentStatusAsync(Order order, OrderStatus orderStatus, PaymentStatus? paymentStatus)
        {
            if (order is null)
                return;

            order.OrderStatus = orderStatus;

            if (paymentStatus is not null)
                order.PaymentStatus = paymentStatus;

            await _unitOfWork.CompleteAsync();
        }
    }
}
