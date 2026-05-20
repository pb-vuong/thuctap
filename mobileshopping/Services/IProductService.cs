
using mobileshopping.DTOs;

namespace mobileshopping.Services
{
    public interface IProductService
   
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto> GetByIdAsync(int id);
        Task CreateAsync(ProductDto dto);
        Task UpdateAsync(int id, ProductDto dto);
        Task DeleteAsync(int id);
    }

    public interface ICartService
    {
        Task<CartDto> GetCartByUserIdAsync(string userId);
        Task AddToCartAsync(string userId, int productId, int quantity);
        Task RemoveFromCartAsync(string userId, int cartItemId);
        Task ClearCartAsync(string userId);
    }

    public interface IOrderService
    {
        Task<OrderDto> CheckoutAsync(string userId);
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);
    }
}
