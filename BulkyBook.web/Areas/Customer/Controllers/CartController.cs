using AutoMapper;
using BulkyBook.BLL.Services.Contract;
using BulkyBook.DAL.InterFaces;
using BulkyBook.Model.Identity;
using BulkyBook.Model.OrdersAggregate;
using BulkyBook.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBook.web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public CartController(
            IShoppingCartService shoppingCartService,
            IOrderService orderService,
            UserManager<AppUser> userManager,
            IMapper mapper
            )
        {
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null)
            {
                var cart = await _shoppingCartService.GetOrCreateCartAsync(userId);
                decimal totalPrice = cart.CartItems
                            .Sum(item => _shoppingCartService.GetPriceBasedOnQuantity(item) * item.Quantity);

                cart.TotalPrice = totalPrice;

                var cartVM = _mapper.Map<ShoppingCartVM>(cart);

                return View(cartVM);
            }

            return NotFound();
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
                var cart = await _shoppingCartService.GetOrCreateCartAsync(userId);
                decimal totalPrice = cart.CartItems
                            .Sum(item => _shoppingCartService.GetPriceBasedOnQuantity(item) * item.Quantity);

                cart.TotalPrice = totalPrice;

                var cartVM = _mapper.Map<ShoppingCartVM>(cart);

                return View(cartVM);
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
            if (userId is not null)
            {
                var cartId = _shoppingCartService.GetShoppingCartAsync(userId).Result?.Id;
                if (cartId is not null)
                {
                    var orderAddress = new OrderAddress(orderVM.OrderAddress.FullName, orderVM.OrderAddress.City, orderVM.OrderAddress.Street, orderVM.OrderAddress.State, orderVM.OrderAddress.PhoneNumber);

                    var orderCreated = await _orderService.CreateOrderAsync(userId, cartId, orderAddress);
                    if (orderCreated is not null)
                    {
                        TempData["success"] = "Order created successfully";
                        await _shoppingCartService.RemoveCartAsync(cartId);
                        return RedirectToAction(nameof(OrderConfirmation), new { orderId = orderCreated.Id });
                    }
                }
            }

            return BadRequest();
        }

        public IActionResult OrderConfirmation(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
                return NotFound();


            return View(nameof(OrderConfirmation), orderId);
        }

    }
}
