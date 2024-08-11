using AutoMapper;
using BulkyBook.BLL.Services.Contract;
using BulkyBook.DAL.InterFaces;
using BulkyBook.Model.OrdersAggregate;
using BulkyBook.Model.ViewModels;
using BulkyBook.Model.ViewModels.OrderVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBook.web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public CartController(
            IShoppingCartService shoppingCartService,
            IOrderService orderService,
            IPaymentService paymentService,
            IMapper mapper
            )
        {
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
            _paymentService = paymentService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null)
            {
                var cart = await _shoppingCartService.GetCartAsync(userId, true, true);
                if (cart is not null)
                {
                    decimal totalPrice = cart.CartItems
                            .Sum(item => _shoppingCartService.GetPriceBasedOnQuantity(item) * item.Quantity);

                    cart.TotalPrice = totalPrice;

                    var cartVM = _mapper.Map<ShoppingCartVM>(cart);

                    return View(cartVM);
                }
            }

            return View(new ShoppingCartVM());
        }

        public async Task<IActionResult> Plus(string cartId)
        {
            await _shoppingCartService.IncrementCartItemAsync(cartId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(string cartId)
        {
            await _shoppingCartService.DecrementCartItemAsync(cartId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(string cartItemId)
        {
            try
            {
                await _shoppingCartService.RemoveCartItemAsync(cartItemId);
                TempData["success"] = "Cart item removed successfully";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while removing the cart: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Summary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null)
            {
                var cart = await _shoppingCartService.GetCartAsync(userId, true, false);

                if (cart is not null)
                {
                    decimal totalPrice = cart.CartItems
                            .Sum(item => _shoppingCartService.GetPriceBasedOnQuantity(item) * item.Quantity);

                    cart.TotalPrice = totalPrice;

                    var cartVM = _mapper.Map<ShoppingCartVM>(cart);

                    return View(cartVM);
                }
            }

            return NotFound();
        }

        public IActionResult PlaceOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderVM orderVM)
        {
            if (!ModelState.IsValid)
                return View(orderVM);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderCreated = new Order();
            if (userId is not null)
            {
                var cart = await _shoppingCartService.GetCartAsync(userId, true);
                if (cart is not null)
                {
                    var orderAddress = new OrderAddress(orderVM.OrderAddress.FullName, orderVM.OrderAddress.City, orderVM.OrderAddress.Street, orderVM.OrderAddress.State, orderVM.OrderAddress.PhoneNumber);

                    var oo = await _orderService.CreateOrderAsync(userId, cart, orderAddress);
                    orderCreated = await _orderService.GetOrderByIdAsync(userId, true);

                    if (orderCreated is not null && orderCreated.AppUser.CompanyId is null)
                    {
                        var session = await _paymentService.CreateSessionPaymentAsync(cart, orderCreated);

                        Response.Headers.Add("Location", session.Url);

                        return new StatusCodeResult(303);
                    }
                }
            }

            return RedirectToAction(nameof(OrderConfirmation), new { orderId = orderCreated?.Id });
        }

        public async Task<IActionResult> OrderConfirmation(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order is null)
                return NotFound();

            if (order.PaymentStatus != PaymentStatus.ApprovedForDelayedPayment)
            {
                // This is an order by customer
                var service = new SessionService();
                Session session = service.Get(order.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _paymentService.UpdatePaymentIntentIdAndSessionIdAsync(order, session.Id, session.PaymentIntentId);
                    await _paymentService.UpdateOrderAndPaymentStatusAsync(order.Id, OrderStatus.Approved, PaymentStatus.Approved);
                }

                //HttpContext.Session.Clear();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var cart = await _shoppingCartService.GetCartAsync(userId);

            if (cart is not null)
                await _shoppingCartService.RemoveCartAsync(cart.Id);

            TempData["success"] = "Order created successfully";

            return View(nameof(OrderConfirmation), orderId);
        }

    }
}
