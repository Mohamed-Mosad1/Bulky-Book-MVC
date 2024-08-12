using BulkyBook.DAL.InterFaces;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBook.web.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {

        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILogger<ShoppingCartViewComponent> _logger;

        public ShoppingCartViewComponent(IShoppingCartService shoppingCartService, ILogger<ShoppingCartViewComponent> logger)
        {
            _shoppingCartService = shoppingCartService;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                int cartCount = 0;

                if (userId is not null)
                {
                    cartCount = await _shoppingCartService.GetCountAsync(userId);

                    HttpContext.Session.SetInt32(SD.SessionCart, cartCount);
                }
                else
                {
                    HttpContext.Session.Clear();
                }

                return View(cartCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ShoppingCartViewComponent");

                return View(0);
            }
        }

    }
}
