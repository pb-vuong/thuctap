using Moq;
using Xunit;
using mobileshopping.Models;
using mobileshopping.Repositories;
using mobileshopping.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace mobileshopping.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IGenericRepository<Cart>> _mockCartRepo;
        private readonly Mock<IGenericRepository<CartItem>> _mockCartItemRepo;
        private readonly Mock<IGenericRepository<Order>> _mockOrderRepo;
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockCartRepo = new Mock<IGenericRepository<Cart>>();
            _mockCartItemRepo = new Mock<IGenericRepository<CartItem>>();
            _mockOrderRepo = new Mock<IGenericRepository<Order>>();
            _mockProductRepo = new Mock<IGenericRepository<Product>>();

            _mockUow.Setup(u => u.Carts).Returns(_mockCartRepo.Object);
            _mockUow.Setup(u => u.CartItems).Returns(_mockCartItemRepo.Object);
            _mockUow.Setup(u => u.Orders).Returns(_mockOrderRepo.Object);
            _mockUow.Setup(u => u.Products).Returns(_mockProductRepo.Object);

            _service = new OrderService(_mockUow.Object);
        }

        [Fact]
        public async Task CreateOrderFromCartAsync_CartHasItems_CreatesOrderAndClearsCart()
        {
            // 1. Arrange
            int userId = 1;
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductID = 10, Quantity = 2 }
            };
            var cart = new Cart
            {
                UserID = userId,
                SubTotal = 100,
                Tax = 10,
                Total = 110,
                CartItems = cartItems
            };
            var product = new Product { ProductID = 10, Price = 50 };

            _mockCartRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems"))
                         .ReturnsAsync(cart);
            _mockProductRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(product);

            // 2. Act
            var result = await _service.CreateOrderFromCartAsync(userId);

            // 3. Assert
            Assert.NotNull(result);
            Assert.Equal("Pending", result.Status);
            Assert.Equal(110, result.TotalAmount);

            // Xác nhận Order đã được tạo
            _mockOrderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);

            // Xác nhận Cart đã được dọn sạch
            _mockCartItemRepo.Verify(r => r.DeleteRange(cartItems), Times.Once);
            Assert.Equal(0, cart.Total);
            _mockCartRepo.Verify(r => r.Update(cart), Times.Once);

            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}