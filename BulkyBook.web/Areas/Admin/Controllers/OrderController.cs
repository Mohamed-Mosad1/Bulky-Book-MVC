// Ignore Spelling: Admin

using AutoMapper;
using BulkyBook.BLL.Services.Contract;
using BulkyBook.Model.OrdersAggregate;
using BulkyBook.Model.ViewModels.OrderVM;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBook.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public OrderController(
            IOrderService orderService,
            IPaymentService paymentService,
            IMapper mapper
            )
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId, true, true);

            var mappedOrder = _mapper.Map<OrderToReturnVM>(order);

            return View(mappedOrder);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> Details(OrderToReturnVM orderToReturnVM)
        {
            var result = await _orderService.UpdateOrderDetailsAsync(orderToReturnVM);

            if (result)
                TempData["success"] = "Order Details Updated Successfully.";
            else
                TempData["error"] = "Something went wrong while updating the order.";

            return RedirectToAction(nameof(Details), new { orderId = orderToReturnVM.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> StartProcessing(OrderToReturnVM orderToReturnVM)
        {
            await _paymentService.UpdateOrderAndPaymentStatusAsync(orderToReturnVM.Id, OrderStatus.Processing, null);
            TempData["Success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderToReturnVM.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> ShipOrder(OrderToReturnVM orderToReturnVM)
        {
            var result = await _orderService.UpdateOrderToShippedAsync(orderToReturnVM);

            if (result)
                TempData["success"] = "Order Shipped Successfully.";
            else
                TempData["error"] = "Something went wrong while updating the order.";

            return RedirectToAction(nameof(Details), new { orderId = orderToReturnVM.Id });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public async Task<IActionResult> CancelOrder(OrderToReturnVM orderToReturnVM)
        {
            var result = await _orderService.UpdateOrderToCancelAsync(orderToReturnVM);

            if (result)
                TempData["success"] = "Order Cancelled Successfully.";
            else
                TempData["error"] = "Something went wrong while cancelling the order.";

            return RedirectToAction(nameof(Details), new { orderId = orderToReturnVM.Id });

        }

        [HttpPost]
        public async Task<IActionResult> CompanyPayNow(OrderToReturnVM orderToReturnVM)
        {
            var session = await _paymentService.CreateSessionPaymentAsync(orderToReturnVM.Id);
            if (session?.Url is not null)
                Response.Headers.Add("Location", session.Url);
            else
            {
                TempData["error"] = "Payment session could not be created. Please try again.";
                return RedirectToAction(nameof(Details), new { orderId = orderToReturnVM.Id });
            }

            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> PaymentConfirmation(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order is null)
                return NotFound();

            if (order.PaymentStatus == PaymentStatus.ApprovedForDelayedPayment)
            {
                // This is an order by company
                var service = new SessionService();
                Session session = service.Get(order.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    await _paymentService.UpdatePaymentIntentIdAndSessionIdAsync(order, session.Id, session.PaymentIntentId);
                    await _paymentService.UpdateOrderAndPaymentStatusAsync(order.Id, order.OrderStatus, PaymentStatus.Approved);
                }
            }

            TempData["success"] = "Payment confirmed successfully";

            return View(nameof(PaymentConfirmation), order.Id);
        }

        #region API CALLS

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderToReturnVM>>> GetAll(string status)
        {
            IReadOnlyList<Order> orders = new List<Order>();
            string? userId;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orders = await _orderService.GetOrdersForUserAsync(includeUser: true);
            }
            else
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userId))
                    orders = await _orderService.GetOrdersForUserAsync(userId, true);
            }

            var mappedOrders = _mapper.Map<IEnumerable<OrderToReturnVM>>(orders);

            switch (status)
            {
                case "pending":
                    mappedOrders = mappedOrders.Where(u =>
                    u.PaymentStatus?.ToString() == PaymentStatus.ApprovedForDelayedPayment.ToString()
                    || u.PaymentStatus?.ToString() == PaymentStatus.Pending.ToString());
                    break;
                case "processing":
                    mappedOrders = mappedOrders.Where(u => u.OrderStatus?.ToString() == OrderStatus.Processing.ToString());
                    break;
                case "completed":
                    mappedOrders = mappedOrders.Where(u => u.OrderStatus?.ToString() == OrderStatus.Shipped.ToString());
                    break;
                case "approved":
                    mappedOrders = mappedOrders.Where(u => u.OrderStatus?.ToString() == OrderStatus.Approved.ToString());
                    break;
                default:
                    break;

            }

            return Json(new { data = mappedOrders });
        }

        #endregion
    }
}
