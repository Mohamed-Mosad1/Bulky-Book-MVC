// Ignore Spelling: BLL

using BulkyBook.BLL.Services.Contract;
using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Specifications.OrderSpecs;
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

        public async Task<Session?> CreateSessionPaymentAsync(string orderId)
        {
            // Get Secret key
            StripeConfiguration.ApiKey = _configuration["Stripe:Secretkey"];

            var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(new OrdersWithOrderItemsSpec(orderId, true, true));

            if (order is null)
                return null;

            var domain = _configuration["BaseUrl"];
            var options = new SessionCreateOptions();
            if (order.AppUser.CompanyId is not null)
            {
                options = new SessionCreateOptions()
                {
                    SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderId={order.Id}",
                    CancelUrl = domain + $"admin/order/details?orderId={order.Id}",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"
                };
            }
            else
            {
                options = new SessionCreateOptions()
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?orderId={order.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"
                };
            }

            if (order.OrderItems.Count > 0)
            {
                foreach (var item in order.OrderItems)
                {
                    var sessionLineItem = new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.ProductOrdered.ProductPrice * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.ProductOrdered.ProductTitle
                            }
                        },
                        Quantity = item.Quantity
                    };
                    options.LineItems.Add(sessionLineItem);
                }
            }

            var sessionService = new SessionService();
            var session = await sessionService.CreateAsync(options);

            await UpdatePaymentIntentIdAndSessionIdAsync(order, session.Id, session.PaymentIntentId);

            await _unitOfWork.CompleteAsync();

            return session;
        }

        public async Task UpdatePaymentIntentIdAndSessionIdAsync(Order order, string sessionId, string? paymentIntentId)
        {
            if (order is not null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                    order.SessionId = sessionId;

                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    order.PaymentIntentId = paymentIntentId;
                    order.PaymentDate = DateTimeOffset.UtcNow;
                }
            }

            _unitOfWork.Repository<Order>().Update(order);
            
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateOrderAndPaymentStatusAsync(string orderId, OrderStatus orderStatus, PaymentStatus? paymentStatus)
        {
            var orderFromDb = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (orderFromDb is not null)
            {
                orderFromDb.OrderStatus = orderStatus;

                if (paymentStatus is not null)
                    orderFromDb.PaymentStatus = paymentStatus;

                _unitOfWork.Repository<Order>().Update(orderFromDb);
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
