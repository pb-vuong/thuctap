using mobileshopping.DTOs;
using mobileshopping.Models;
using mobileshopping.Repositories;

namespace mobileshopping.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDto?> GetCartByUserIdAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetFirstOrDefaultAsync(c => c.UserID == userId, "CartItems");
            if (cart == null) return null;

            return new CartDto
            {
                Id = cart.CartID,
                UserId = cart.UserID.ToString(),
                TotalPrice = cart.Total,
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    Id = ci.CartItemID,
                    ProductId = ci.ProductID,
                    Quantity = ci.Quantity
                }).ToList()
            };
        }

        public async Task<bool> AddToCartAsync(int userId, int productId, int quantity)
        {
            var cart = await _unitOfWork.Carts.GetFirstOrDefaultAsync(c => c.UserID == userId, "CartItems");

            // 1. Tạo giỏ hàng nếu user chưa có
            if (cart == null)
            {
                cart = new Cart { UserID = userId, SubTotal = 0, Tax = 0, Total = 0 };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveAsync();
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null) return false;

            // 2. Kiểm tra xem sản phẩm đã có trong giỏ chưa
            var existingItem = cart.CartItems?.FirstOrDefault(ci => ci.ProductID == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _unitOfWork.CartItems.Update(existingItem);
            }
            else
            {
                var newItem = new CartItem { CartID = cart.CartID, ProductID = productId, Quantity = quantity };
                await _unitOfWork.CartItems.AddAsync(newItem);
            }

            // 3. Cập nhật tổng tiền (Giả sử Total = SubTotal)
            cart.Total += (product.Price * quantity);
            _unitOfWork.Carts.Update(cart);

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> RemoveItemAsync(int cartItemId)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
            if (cartItem == null) return false;

            var cart = await _unitOfWork.Carts.GetByIdAsync(cartItem.CartID);
            var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductID);

            if (cart != null && product != null)
            {
                cart.Total -= (product.Price * cartItem.Quantity);
                _unitOfWork.Carts.Update(cart);
            }

            _unitOfWork.CartItems.Delete(cartItem);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cart = await _unitOfWork.Carts.GetFirstOrDefaultAsync(c => c.UserID == userId, "CartItems");
            if (cart == null) return false;

            if (cart.CartItems != null && cart.CartItems.Any())
            {
                _unitOfWork.CartItems.DeleteRange(cart.CartItems);
            }

            cart.Total = 0;
            cart.SubTotal = 0;
            _unitOfWork.Carts.Update(cart);

            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}