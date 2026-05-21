using mobileshopping.DTOs;
using mobileshopping.Models;
using mobileshopping.Repositories;

namespace mobileshopping.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderDto?> CreateOrderFromCartAsync(int userId)
        {
            // 1. Lấy giỏ hàng của User
            var cart = await _unitOfWork.Carts.GetFirstOrDefaultAsync(c => c.UserID == userId, "CartItems");
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return null; 

            // 2. Tạo Đơn hàng mới
            var order = new Order
            {
                UserID = userId,
                OrderDate = DateTime.Now,
                SubTotal = cart.SubTotal,
                Tax = cart.Tax,
                Total = cart.Total,
                OrderItems = new List<OrderItem>()
            };

            // 3. Chuyển CartItem sang OrderItem
            foreach (var item in cart.CartItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductID);
                if (product != null)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        Price = product.Price 
                    });
                }
            }

            await _unitOfWork.Orders.AddAsync(order);

            // 4. Xóa các sản phẩm trong giỏ hàng sau khi đặt hàng thành công
            _unitOfWork.CartItems.DeleteRange(cart.CartItems);
            cart.Total = 0;
            _unitOfWork.Carts.Update(cart);

            await _unitOfWork.SaveAsync();

            return new OrderDto
            {
                Id = order.OrderID,
                UserId = userId.ToString(),
                OrderDate = order.OrderDate,
                TotalAmount = order.Total,
                Status = "Pending"
            };
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _unitOfWork.Orders.GetAllAsync("OrderItems");

            return orders.Where(o => o.UserID == userId).Select(o => new OrderDto
            {
                Id = o.OrderID,
                UserId = o.UserID.ToString(),
                OrderDate = o.OrderDate,
                TotalAmount = o.Total,
                Status = "Completed", 
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductID,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Price
                }).ToList()
            });
        }
    }
}