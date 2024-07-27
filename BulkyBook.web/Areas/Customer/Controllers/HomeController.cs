using AutoMapper;
using BulkyBook.BLL.Services.Contract;
using BulkyBook.Model;
using BulkyBook.Model.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyBook.web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public HomeController(
            ILogger<HomeController> logger, 
            IProductService productService,
            IMapper mapper
            )
        {
            _logger = logger;
            _productService = productService;
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
