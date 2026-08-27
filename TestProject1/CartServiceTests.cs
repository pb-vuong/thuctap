using Moq;
using Xunit;
using mobileshopping.Models;
using mobileshopping.Repositories;
using mobileshopping.Services;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace mobileshopping.Tests
{
    public class CartServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IGenericRepository<Cart>> _mockCartRepo;
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo;
        private readonly Mock<IGenericRepository<CartItem>> _mockCartItemRepo;
        private readonly CartService _service;

        public CartServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockCartRepo = new Mock<IGenericRepository<Cart>>();
            _mockProductRepo = new Mock<IGenericRepository<Product>>();
            _mockCartItemRepo = new Mock<IGenericRepository<CartItem>>();

            _mockUow.Setup(u => u.Carts).Returns(_mockCartRepo.Object);
            _mockUow.Setup(u => u.Products).Returns(_mockProductRepo.Object);
            _mockUow.Setup(u => u.CartItems).Returns(_mockCartItemRepo.Object);

            _service = new CartService(_mockUow.Object);
        }

        [Fact]
        public async Task AddToCartAsync_ProductNotFound_ReturnsFalse()
        {
            // 1. Arrange
            int userId = 1, productId = 999, quantity = 1;
            var cart = new Cart { CartID = 1, UserID = userId, CartItems = new List<CartItem>() };

            _mockCartRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems"))
                         .ReturnsAsync(cart);

            _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
                            .ReturnsAsync((Product?)null);

            // 2. Act
            var result = await _service.AddToCartAsync(userId, productId, quantity);

            // 3. Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddToCartAsync_NewItem_CalculatesTotalCorrectly()
        {
            // 1. Arrange
            int userId = 1, productId = 10, quantity = 2;
            decimal productPrice = 500000;

            var cart = new Cart
            {
                CartID = 1,
                UserID = userId,
                Total = 0,
                CartItems = new List<CartItem>()
            };
            var product = new Product { ProductID = productId, Price = productPrice };

            _mockCartRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems"))
                         .ReturnsAsync(cart);
            _mockProductRepo.Setup(r => r.GetByIdAsync(productId))
                            .ReturnsAsync(product);

            // 2. Act
            var result = await _service.AddToCartAsync(userId, productId, quantity);

            // 3. Assert
            Assert.True(result);
            Assert.Equal(1000000, cart.Total); // 500,000 * 2 = 1,000,000
            _mockCartItemRepo.Verify(r => r.AddAsync(It.IsAny<CartItem>()), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveItemAsync_WhenItemExists_RemovesAndDeductsTotal()
        {
            // 1. Arrange
            int cartItemId = 1;
            var cartItem = new CartItem { CartItemID = cartItemId, CartID = 1, ProductID = 10, Quantity = 2 };
            var cart = new Cart { CartID = 1, Total = 1000000 }; // Đang có 1 triệu
            var product = new Product { ProductID = 10, Price = 500000 };

            _mockCartItemRepo.Setup(r => r.GetByIdAsync(cartItemId)).ReturnsAsync(cartItem);
            _mockCartRepo.Setup(r => r.GetByIdAsync(cartItem.CartID)).ReturnsAsync(cart);
            _mockProductRepo.Setup(r => r.GetByIdAsync(cartItem.ProductID)).ReturnsAsync(product);

            // 2. Act
            var result = await _service.RemoveItemAsync(cartItemId);

            // 3. Assert
            Assert.True(result);
            Assert.Equal(0, cart.Total); // 1,000,000 - (500,000 * 2) = 0
            _mockCartItemRepo.Verify(r => r.Delete(cartItem), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task ClearCartAsync_WhenCartExists_DeletesAllItemsAndResetsTotal()
        {
            // 1. Arrange
            int userId = 1;
            var cartItems = new List<CartItem>
    {
        new CartItem { CartItemID = 1, ProductID = 1 }
    };
            var cart = new Cart { CartID = 1, UserID = userId, Total = 500000, SubTotal = 500000, CartItems = cartItems };

            _mockCartRepo.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<Cart, bool>>>(), "CartItems"))
                         .ReturnsAsync(cart);

            // 2. Act
            var result = await _service.ClearCartAsync(userId);

            // 3. Assert
            Assert.True(result);
            Assert.Equal(0, cart.Total);
            Assert.Equal(0, cart.SubTotal);
            _mockCartItemRepo.Verify(r => r.DeleteRange(cartItems), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}