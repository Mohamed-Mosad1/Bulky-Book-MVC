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

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId is not null)
                {
                    var cart = await _shoppingCartService.GetOrCreateCartAsync(userId);

                    var user = await _userManager.FindByIdAsync(userId);

                    var orderAddress = new OrderAddress(user.UserName, user.City, user.Street, user.State, user.PhoneNumber);

                    var orderCreated = await _orderService.CreateOrderAsync(userId, cart.Id, orderAddress);

                    return View();
                }
            }

            return NotFound();
        }

    }
}
