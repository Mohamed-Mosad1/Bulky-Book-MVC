// Ignore Spelling: BLL

using BulkyBook.BLL.Services.Contract;
using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Specifications.OrderSpecs;
using BulkyBook.DAL.Specifications.ProductSpecs;
using BulkyBook.Model.Cart;
using BulkyBook.Model.OrdersAggregate;
using BulkyBook.Model.ViewModels.OrderVM;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace BulkyBook.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public OrderService(
            IShoppingCartService shoppingCartService,
            IPaymentService paymentService,
            IConfiguration configuration,
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
            _shoppingCartService = shoppingCartService;
            _paymentService = paymentService;
            _configuration = configuration;
        }

        public async Task<Order?> CreateOrderAsync(string userId, ShoppingCart shoppingCart, OrderAddress orderAddress)
        {
            var orderItems = new List<OrderItem>();

            if (shoppingCart.CartItems.Count > 0)
            {
                foreach (var item in shoppingCart.CartItems)
                {
                    var spec = new ProductWithImagesSpecification(item.ProductId);
                    var product = await _unitOfWork.Repository<Model.Product>().GetWithSpecAsync(spec);
                    var productItemOrdered = new ProductItemOrdered();
                    if (product is not null)
                    {
                        productItemOrdered = new ProductItemOrdered(product.Id, product.Title, product.ProductImages.FirstOrDefault()?.ImageUrl);
                        var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
                        orderItems.Add(orderItem);
                    }
                }
            }

            var orderTotal = shoppingCart.CartItems
                            .Sum(item => _shoppingCartService.GetPriceBasedOnQuantity(item) * item.Quantity);

            // It is a regular customer
            var orderStatus = OrderStatus.Pending;
            var paymentStatus = PaymentStatus.Pending;

            if (shoppingCart.AppUser.CompanyId > 0)
            {
                // It is a company user
                orderStatus = OrderStatus.Approved;
                paymentStatus = PaymentStatus.ApprovedForDelayedPayment;

            }

            var orderSpec = new OrderWithPaymentIntentSpec(shoppingCart.LastSessionId);
            var existOrder = await _unitOfWork.Repository<Order>().GetWithSpecAsync(orderSpec);
            if (existOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(existOrder);
            }

            var order = new Order(userId, orderStatus, orderAddress, orderItems, orderTotal, paymentStatus: paymentStatus);

            //await _paymentService.CreateOrUpdatePaymentIntentAsync(shoppingCart, orderTotal, order.Id);

            _unitOfWork.Repository<Order>().Add(order);

            var result = await _unitOfWork.CompleteAsync();

            return result > 0 ? order : null;
        }

        public async Task<Order?> GetOrderByIdAsync(string orderId, bool includeUser = false, bool includeOrderItems = false)
        {
            var spec = new OrdersWithOrderItemsSpec(orderId, includeUser, includeOrderItems);
            return await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string? userId = null, bool includeUser = false, bool includeOrderItems = false)
        {
            var spec = new OrdersWithOrderItemsSpec(userId, includeUser, includeOrderItems);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
            return orders;
        }

        public async Task<bool> UpdateOrderDetailsAsync(OrderToReturnVM orderToReturnVM)
        {
            var orderFromDb = await _unitOfWork.Repository<Order>().GetByIdAsync(orderToReturnVM.Id);
            if (orderFromDb is null)
                return false;

            orderFromDb.OrderAddress.FullName = orderToReturnVM.OrderAddress.FullName;
            orderFromDb.OrderAddress.PhoneNumber = orderToReturnVM.OrderAddress.PhoneNumber;
            orderFromDb.OrderAddress.Street = orderToReturnVM.OrderAddress.Street;
            orderFromDb.OrderAddress.City = orderToReturnVM.OrderAddress.City;
            orderFromDb.OrderAddress.State = orderToReturnVM.OrderAddress.State;

            if (!string.IsNullOrEmpty(orderToReturnVM.Carrier))
                orderFromDb.Carrier = orderToReturnVM.Carrier;

            if (!string.IsNullOrEmpty(orderToReturnVM.TrackingNumber))
                orderFromDb.TrackingNumber = orderToReturnVM.TrackingNumber;

            if (orderToReturnVM.OrderStatus == OrderStatus.Processing.ToString())
                orderFromDb.OrderStatus = OrderStatus.Shipped;

            if (orderToReturnVM.PaymentStatus == PaymentStatus.ApprovedForDelayedPayment.ToString())
            {
                orderFromDb.ShippingDate = DateTimeOffset.UtcNow;
                orderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
            }


            _unitOfWork.Repository<Order>().Update(orderFromDb);
            var result = await _unitOfWork.CompleteAsync();

            return result > 0;
        }

        public async Task<bool> UpdateOrderToShippedAsync(OrderToReturnVM orderToReturnVM)
        {
            var orderFromDb = await _unitOfWork.Repository<Order>().GetByIdAsync(orderToReturnVM.Id);
            if (orderFromDb is null)
                return false;

            if (!string.IsNullOrEmpty(orderToReturnVM.Carrier))
                orderFromDb.Carrier = orderToReturnVM.Carrier;

            if (!string.IsNullOrEmpty(orderToReturnVM.TrackingNumber))
                orderFromDb.TrackingNumber = orderToReturnVM.TrackingNumber;

            orderFromDb.OrderStatus = OrderStatus.Shipped;
            orderFromDb.ShippingDate = DateTimeOffset.UtcNow;

            if (orderToReturnVM.PaymentStatus == PaymentStatus.ApprovedForDelayedPayment.ToString())
                orderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

            _unitOfWork.Repository<Order>().Update(orderFromDb);
            var result = await _unitOfWork.CompleteAsync();

            return result > 0;
        }

        public async Task<bool> UpdateOrderToCancelAsync(OrderToReturnVM orderToReturnVM)
        {
            // Get Secret key
            StripeConfiguration.ApiKey = _configuration["Stripe:Secretkey"];

            var orderFromDb = await _unitOfWork.Repository<Order>().GetByIdAsync(orderToReturnVM.Id);
            if (orderFromDb is null)
                return false;

            if (orderFromDb.PaymentStatus == PaymentStatus.Approved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderFromDb.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                await _paymentService.UpdateOrderAndPaymentStatusAsync(orderFromDb.Id, OrderStatus.Cancelled, PaymentStatus.Refunded);
            }
            else
            {
                await _paymentService.UpdateOrderAndPaymentStatusAsync(orderFromDb.Id, OrderStatus.Cancelled, PaymentStatus.Cancelled);
            }

            _unitOfWork.Repository<Order>().Update(orderFromDb);
            var result = await _unitOfWork.CompleteAsync();

            return result > 0;
        }

    }
}
