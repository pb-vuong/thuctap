using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mobileshopping.Services;

namespace mobileshopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/Carts/5
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound(new { message = "Giỏ hàng trống hoặc không tồn tại." });
            }
            return Ok(cart);
        }

        // POST: api/Carts/5/add?productId=1&quantity=2
        [HttpPost("{userId}/add")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> AddToCart(int userId, [FromQuery] int productId, [FromQuery] int quantity = 1)
        {
            if (quantity <= 0)
            {
                return BadRequest(new { message = "Số lượng thêm vào phải lớn hơn 0." });
            }

            var result = await _cartService.AddToCartAsync(userId, productId, quantity);
            if (!result)
            {
                return BadRequest(new { message = "Không thể thêm vào giỏ hàng. Hãy kiểm tra lại ID sản phẩm." });
            }
            return Ok(new { message = "Đã thêm sản phẩm vào giỏ hàng." });
        }

        // DELETE: api/Carts/items/5
        [HttpDelete("items/{cartItemId}")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var result = await _cartService.RemoveItemAsync(cartItemId);
            if (!result)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm này trong giỏ hàng." });
            }
            return Ok(new { message = "Đã xóa sản phẩm khỏi giỏ hàng thành công." });
        }

        // DELETE: api/Carts/5/clear
        [HttpDelete("{userId}/clear")]
        [Authorize(Roles = "user")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            var result = await _cartService.ClearCartAsync(userId);
            if (!result)
            {
                return NotFound(new { message = "Không thể xóa hoặc giỏ hàng đã trống sẵn." });
            }
            return Ok(new { message = "Đã dọn dẹp sạch toàn bộ giỏ hàng." });
        }
    }
}