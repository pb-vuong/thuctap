
using mobileshopping.DTOs;

namespace mobileshopping.Services
{
    public interface IProductService
   
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> AddAsync(ProductDto dto);
        Task<bool> UpdateAsync(int id, ProductDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public interface ICartService
    {
        Task<CartDto?> GetCartByUserIdAsync(int userId);
        Task<bool> AddToCartAsync(int userId, int productId, int quantity);
        Task<bool> RemoveItemAsync(int cartItemId);
        Task<bool> ClearCartAsync(int userId);
    }

    public interface IOrderService
    {
        Task<OrderDto?> CreateOrderFromCartAsync(int userId);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId);
    }
}
