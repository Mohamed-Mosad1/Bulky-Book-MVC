// Ignore Spelling: BLL

using BulkyBook.BLL.Services.Contract;
using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Specifications.OrderSpecs;
using BulkyBook.DAL.Specifications.ProductSpecs;
using BulkyBook.Model;
using BulkyBook.Model.Cart;
using BulkyBook.Model.OrdersAggregate;

namespace BulkyBook.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPaymentService _paymentService;

        public OrderService(
            IShoppingCartService shoppingCartService,
            IPaymentService paymentService,
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
            _shoppingCartService = shoppingCartService;
            _paymentService = paymentService;
        }

        public async Task<Order?> CreateOrderAsync(string userId, ShoppingCart shoppingCart, OrderAddress orderAddress)
        {
            var orderItems = new List<OrderItem>();

            if (shoppingCart.CartItems.Count > 0)
            {
                foreach (var item in shoppingCart.CartItems)
                {
                    var spec = new ProductWithImagesSpecification(item.ProductId);
                    var product = await _unitOfWork.Repository<Product>().GetWithSpecAsync(spec);
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

            if (shoppingCart.AppUser.CompanyId.GetValueOrDefault() > 0)
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

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string userId)
        {
            var spec = new OrdersWithOrderItemsSpec(userId);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
            return orders;
        }
    }
}
