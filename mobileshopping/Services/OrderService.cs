using mobileshopping.DTOs;
using mobileshopping.Models;
using mobileshopping.Repositories;

namespace mobileshopping.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;

        public OrderService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrderDto> CreateOrderFromCartAsync(string userId)
        {
            // 1. Lấy giỏ hàng của user
            var cart = await _uow.Carts.GetFirstOrDefaultAsync(c => c.UserId == userId, includeProperties: "CartItems.Product");
            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Giỏ hàng trống!");

            // 2. Tính tổng tiền
            decimal totalAmount = cart.CartItems.Sum(item => item.Quantity * item.Product.Price);

            // 3. Tạo Đơn hàng (Order)
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = totalAmount,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.Price
                }).ToList()
            };

            await _uow.Orders.AddAsync(order);

            // 4. Xóa giỏ hàng sau khi đặt hàng thành công
            _uow.CartItems.RemoveRange(cart.CartItems);

            // 5. Lưu vào database (Sử dụng Transaction của UoW)
            await _uow.SaveAsync();

            return new OrderDto { Id = order.Id, TotalAmount = order.TotalAmount, Status = order.Status };
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _uow.Orders.GetAllAsync(o => o.UserId == userId, includeProperties: "OrderItems.Product");

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            });
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            var order = await _uow.Orders.GetFirstOrDefaultAsync(o => o.Id == orderId, includeProperties: "OrderItems.Product");
            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
    }
}
