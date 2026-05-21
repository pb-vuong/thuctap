using Microsoft.AspNetCore.Mvc;
using mobileshopping.Services;

namespace mobileshopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: api/Orders/checkout/5
        [HttpPost("checkout/{userId}")]
        public async Task<IActionResult> Checkout(int userId)
        {
            var order = await _orderService.CreateOrderFromCartAsync(userId);
            if (order == null)
            {
                return BadRequest(new { message = "Thanh toán thất bại. Giỏ hàng của bạn đang trống hoặc không tồn tại." });
            }
            return Ok(order);
        }

        // GET: api/Orders/user/5
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }
    }
}