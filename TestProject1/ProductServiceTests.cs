using Moq;
using Xunit;
using mobileshopping.Models;
using mobileshopping.Repositories;
using mobileshopping.Services;
using mobileshopping.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace mobileshopping.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            // Khởi tạo mock cho IUnitOfWork và IGenericRepository<Product>
            _mockUow = new Mock<IUnitOfWork>();
            _mockProductRepo = new Mock<IGenericRepository<Product>>();

            // Cấu hình UoW.Products trả về repo giả lập
            _mockUow.Setup(u => u.Products).Returns(_mockProductRepo.Object);

            // Khởi tạo Service với Mock UoW
            _service = new ProductService(_mockUow.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WhenProductExists_ReturnsProductDto()
        {
            // 1. Arrange
            int productId = 1;
            var fakeProduct = new Product
            {
                ProductID = productId,
                ProductName = "iPhone 15",
                Price = 20000000,
                Description = "Apple Phone",
                CategoryID = 1
            };

            _mockProductRepo.Setup(repo => repo.GetByIdAsync(productId))
                            .ReturnsAsync(fakeProduct);

            // 2. Act
            var result = await _service.GetByIdAsync(productId);

            // 3. Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("iPhone 15", result.Name);
            Assert.Equal(20000000, result.Price);
        }

        [Fact]
        public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
        {
            // 1. Arrange
            int productId = 999;
            _mockProductRepo.Setup(repo => repo.GetByIdAsync(productId))
                            .ReturnsAsync((Product?)null);

            // 2. Act
            var result = await _service.GetByIdAsync(productId);

            // 3. Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ValidDto_AddsProductAndCallsSave()
        {
            // 1. Arrange
            var newDto = new ProductDto
            {
                Name = "Samsung Galaxy S24",
                Price = 18000000,
                Description = "Samsung Flagship",
                CategoryId = 2
            };

            _mockProductRepo.Setup(repo => repo.AddAsync(It.IsAny<Product>()))
                            .Returns(Task.CompletedTask);
            _mockUow.Setup(u => u.SaveAsync())
                    .ReturnsAsync(1);

            // 2. Act
            var result = await _service.AddAsync(newDto);

            // 3. Assert
            _mockProductRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
            Assert.Equal("Samsung Galaxy S24", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_WhenProductExists_UpdatesAndReturnsTrue()
        {
            // 1. Arrange
            int productId = 1;
            var existingProduct = new Product { ProductID = productId, ProductName = "Old Name" };
            var updateDto = new ProductDto { Name = "New Name", Price = 15000000 };

            _mockProductRepo.Setup(repo => repo.GetByIdAsync(productId))
                            .ReturnsAsync(existingProduct);

            // 2. Act
            var result = await _service.UpdateAsync(productId, updateDto);

            // 3. Assert
            Assert.True(result);
            Assert.Equal("New Name", existingProduct.ProductName); // Đảm bảo dữ liệu đã được gán lại
            _mockProductRepo.Verify(r => r.Update(existingProduct), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenProductExists_DeletesAndReturnsTrue()
        {
            // 1. Arrange
            int productId = 1;
            var existingProduct = new Product { ProductID = productId };

            _mockProductRepo.Setup(repo => repo.GetByIdAsync(productId))
                            .ReturnsAsync(existingProduct);

            // 2. Act
            var result = await _service.DeleteAsync(productId);

            // 3. Assert
            Assert.True(result);
            _mockProductRepo.Verify(r => r.Delete(existingProduct), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}