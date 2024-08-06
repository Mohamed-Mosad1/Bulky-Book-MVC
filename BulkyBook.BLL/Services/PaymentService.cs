// Ignore Spelling: BLL

using BulkyBook.BLL.Services.Contract;
using BulkyBook.DAL.InterFaces;
using BulkyBook.DAL.Specifications.OrderSpecs;
using BulkyBook.Model.Cart;
using BulkyBook.Model.OrdersAggregate;
using Microsoft.Extensions.Configuration;
using Stripe;

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

        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
        {
            // Get Secret key
            StripeConfiguration.ApiKey = _configuration["Stripe:Secretkey"];
            // Get Basket
            var cart = await _shoppingCartService.GetCartAsync(cartId);
            if (cart is null) return null;

            if (cart.CartItems.Count > 0)
            {
                foreach (var item in cart.CartItems)
                {
                    var product = cart.CartItems.FirstOrDefault(x => x.ProductId == item.ProductId)?.Product;
                    if (product is not null)
                    {
                        if (item.Product.Price != product.Price)
                        {
                            item.Product.Price = product.Price;
                        }
                    }
                }
            }

            var orderTotal = cart.CartItems.Sum(item => _shoppingCartService.GetPriceBasedOnQuantity(item) * item.Quantity);

            // Create Payment Intent
            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(cart.PaymentIntentId)) // Create
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(orderTotal * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                paymentIntent = await service.CreateAsync(options);
                cart.PaymentIntentId = paymentIntent.Id;
                cart.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(orderTotal * 100)
                };

                paymentIntent = await service.UpdateAsync(cart.PaymentIntentId, options);
                cart.PaymentIntentId = paymentIntent.Id;
                cart.ClientSecret = paymentIntent.ClientSecret;
            }

            //await _shoppingCartService.AddOrUpdateToCartAsync(cart);

            return cart;
        }

        public async Task<Order> UpdatePaymentIntentToSucceedOrFailed(string paymentIntentId, bool flag)
        {
            var spec = new OrderWithPaymentIntentSpec(paymentIntentId);

            var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
            if (flag)
            {
                order.PaymentStatus = PaymentStatus.Approved;
            }
            else
            {
                order.PaymentStatus = ~PaymentStatus.Rejected;
            }

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();

            return order;
        }
    }
}
