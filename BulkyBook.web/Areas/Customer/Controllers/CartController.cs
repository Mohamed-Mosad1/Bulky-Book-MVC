﻿using BulkyBook.DAL.InterFaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBook.web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public CartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
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

                return View(cart);
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
                TempData["success"] = "Cart removed successfully";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occurred while removing the cart: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
