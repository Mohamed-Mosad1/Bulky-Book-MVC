// Ignore Spelling: Admin

using AutoMapper;
using BulkyBook.BLL.Services.Contract;
using BulkyBook.Model.ViewModels.OrderVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBook.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            var mappedOrder = _mapper.Map<OrderToReturnVM>(order);

            return View(mappedOrder);
        }

        #region API CALLS

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnVM>>> GetAll(string status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null)
            {
                var orders = await _orderService.GetOrdersForUserAsync(userId);

                var mappedProducts = _mapper.Map<IReadOnlyList<OrderToReturnVM>>(orders);

                return Json(new { data = mappedProducts });
            }

            return NotFound("User not found");
        }

        #endregion
    }
}
