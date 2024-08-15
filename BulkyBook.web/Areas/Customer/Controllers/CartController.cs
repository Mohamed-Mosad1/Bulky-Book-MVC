using AutoMapper;
using BulkyBook.BLL.Services.Contract;
using BulkyBook.DAL.Data;
using BulkyBook.DAL.InterFaces;
using BulkyBook.Model.Cart;
using BulkyBook.Model.OrdersAggregate;
using BulkyBook.Model.ViewModels;
using BulkyBook.Model.ViewModels.OrderVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CartController(
            IShoppingCartService shoppingCartService,
            IOrderService orderService,
            IPaymentService paymentService,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork,
            IMapper mapper
            )
        {
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
            _paymentService = paymentService;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null)
            {
                var cart = await _shoppingCartService.GetCartAsync(userId, true, true, true);
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
                var cart = await _shoppingCartService.GetCartAsync(userId, true, true);

                if (cart is not null)
                {
                    var orderAddress = new OrderAddress(orderVM.OrderAddress.FullName, orderVM.OrderAddress.City, orderVM.OrderAddress.Street, orderVM.OrderAddress.State, orderVM.OrderAddress.PhoneNumber);

                    orderCreated = await _orderService.CreateOrderAsync(userId, cart, orderAddress);
                    Session? session = null!;

                    if (orderCreated is not null && orderCreated.AppUser.CompanyId is null)
                    {
                        session = await _paymentService.CreateSessionPaymentAsync(orderCreated.Id);
                        orderCreated.SessionId = session?.Id;

                        _unitOfWork.Repository<Order>().Update(orderCreated);

                        cart.LastSessionId = session?.Id;
                        _unitOfWork.Repository<ShoppingCart>().Update(cart);

                        await _unitOfWork.CompleteAsync();

                        Response.Headers.Add("Location", session.Url);

                        return new StatusCodeResult(303);
                    }
                }
            }
            return RedirectToAction(nameof(OrderConfirmation), new { orderId = orderCreated?.Id });
        }

        public async Task<IActionResult> OrderConfirmation(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId, true, true);

            if (order is null)
                return NotFound();

            if (order.PaymentStatus != PaymentStatus.ApprovedForDelayedPayment)
            {
                // This is an order by customer
                var service = new SessionService();
                Session session = service.Get(order.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    await _paymentService.UpdatePaymentIntentIdAndSessionIdAsync(order, session.Id, session.PaymentIntentId);
                    await _paymentService.UpdateOrderAndPaymentStatusAsync(order.Id, OrderStatus.Approved, PaymentStatus.Approved);
                }

                //HttpContext.Session.Clear();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var cart = await _shoppingCartService.GetCartAsync(userId);

            if (cart is not null)
            {
                string emailMessage = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .container {{
            width: 100%;
            padding: 20px;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            margin: 20px auto;
            max-width: 600px;
        }}
        .header {{
            text-align: center;
            padding-bottom: 20px;
            border-bottom: 1px solid #eeeeee;
        }}
        .order-details {{
            margin: 20px 0;
        }}
        .order-details h2 {{
            font-size: 20px;
            color: #333333;
        }}
        .order-summary {{
            padding: 10px;
            border-top: 1px solid #eeeeee;
        }}
        .footer {{
            text-align: center;
            color: #777777;
            font-size: 12px;
            padding-top: 20px;
            border-top: 1px solid #eeeeee;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>New Order Confirmation</h1>
            <p>Thank you for shopping with Bulky Book Store!</p>
        </div>
        <div class='order-details'>
            <h2>Your Order Details</h2>
            <p>Order Number: <strong>{order.Id}</strong></p>
            <p>Order Date: {order.OrderDate.ToString("MMMM dd, yyyy")}</p>
            <p>Order Total: ${order.OrderTotal}</p>
        </div>
        <div class='order-summary'>
            <h3>Items Ordered</h3>
            <ul>
                {string.Join("", order.OrderItems.Select(item => $"<li>{item.Quantity}x {item.ProductOrdered.ProductTitle} - ${item.Price}</li>"))}
            </ul>
        </div>
        <div class='footer'>
            <p>If you have any questions, please contact our customer service.</p>
            <p>&copy; {DateTime.Now.Year} Bulky Book Store. All rights reserved.</p>
        </div>
    </div>
</body>
</html>
";

                await _shoppingCartService.RemoveCartAsync(cart.Id);

                await _emailSender.SendEmailAsync(order.AppUser.Email, "New Order - Bulky Book Store", emailMessage);
            }

            TempData["success"] = "Order created successfully";

            return View(nameof(OrderConfirmation), orderId);
        }

    }
}
