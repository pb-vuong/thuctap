using Microsoft.AspNetCore.Mvc;
using mobileshopping.DTOs;
using mobileshopping.Services;

namespace mobileshopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm này." });
            }
            return Ok(product);
        }

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productService.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updated = await _productService.UpdateAsync(id, dto);
            if (!updated)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm để cập nhật." });
            }
            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm để xóa." });
            }
            return Ok(new { message = "Xóa sản phẩm thành công." });
        }
    }
}