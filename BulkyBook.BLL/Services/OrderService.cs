﻿// Ignore Spelling: BLL

using BulkyBook.BLL.Services.Contract;
using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Specifications.OrderSpecs;
using BulkyBook.Model;
using BulkyBook.Model.OrdersAggregate;

namespace BulkyBook.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShoppingCartService _shoppingCartService;

        public OrderService(IUnitOfWork unitOfWork, IShoppingCartService shoppingCartService)
        {
            _unitOfWork = unitOfWork;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<Order?> CreateOrderAsync(string userId, string cartId, OrderAddress orderAddress)
        {
            var cart = await _shoppingCartService.GetCartAsync(userId);
            if (cart is null) return null;

            var orderItems = new List<OrderItem>();

            if (cart.CartItems.Count > 0)
            {
                foreach (var item in cart.CartItems)
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                    var productItemOrdered = new ProductItemOrdered(product.Id, product.Title, product.ProductImages.FirstOrDefault()?.ImageUrl);
                    var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
                    orderItems.Add(orderItem);
                }
            }

            var orderStatus = OrderStatus.Pending;
            var paymentStatus = PaymentStatus.Pending;

            if (cart.AppUser.CompanyId.GetValueOrDefault() > 0)
            {
                orderStatus = OrderStatus.Approved;
                paymentStatus = PaymentStatus.ApprovedForDelayedPayment;

            }

            var orderTotal = orderItems.Sum(item => item.Price * item.Quantity);

            //var orderSpec = new OrderWithPaymentIntentSpec(cart.PaymentIntentId);
            //var existOrder = await _unitOfWork.Repository<Order>().GetWithSpecAsync(orderSpec);
            //if (existOrder is not null)
            //{
            //    _unitOfWork.Repository<Order>().Delete(existOrder);
            //}

            var order = new Order(userId, orderStatus, orderAddress, orderItems, orderTotal, sessionId: null, paymentIntentId: cart.PaymentIntentId, paymentStatus: paymentStatus);

            _unitOfWork.Repository<Order>().Add(order);

            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return null;

            return order;
        }

        public async Task<Order?> GetOrderByIdAsync(string orderId)
        {
            var spec = new OrdersWithOrderItemsSpec(orderId);
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
