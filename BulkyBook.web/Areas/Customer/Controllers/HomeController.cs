using AutoMapper;
using BulkyBook.BLL.Services.Contract;
using BulkyBook.DAL.InterFaces;
using BulkyBook.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBook.web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IMapper _mapper;

        public HomeController(
            IProductService productService,
            IShoppingCartService shoppingCartService,
            IMapper mapper,
            ILogger<HomeController> logger
            )
        {
            _logger = logger;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();

            var productsVM = _mapper.Map<IReadOnlyList<ProductVM>>(products);

            return View(productsVM);
        }

        public async Task<IActionResult> Details(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            var productVM = _mapper.Map<ProductVM>(product);

            return View(productVM);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details([FromQuery] int productId, int quantity)
        {
            //if (productId != 0)
            //    shoppingCartItem.ProductId = productId;

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is not null)
            {
                await _shoppingCartService.AddOrUpdateToCartAsync(userId, productId, quantity);

                TempData["success"] = "Cart updated successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
