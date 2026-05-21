using mobileshopping.DTOs;
using mobileshopping.Models;
using mobileshopping.Repositories;

namespace mobileshopping.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync(null);
            return products.Select(p => new ProductDto
            {
                Id = p.ProductID,           // Sửa thành ProductID
                Name = p.ProductName,      // Sửa thành ProductName
                Description = p.Description,
                Price = p.Price,
                // Stock = p.Stock,        // Xóa dòng này vì Model không có Stock
                ImageURL = p.ImageURL,     // Sửa thành ImageURL
                CategoryId = p.CategoryID  // Sửa thành CategoryID
            });
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var p = await _unitOfWork.Products.GetByIdAsync(id);
            if (p == null) return null;

            return new ProductDto
            {
                Id = p.ProductID,
                Name = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                ImageURL= p.ImageURL,
                CategoryId = p.CategoryID
            };
        }

        public async Task<ProductDto> AddAsync(ProductDto dto)
        {
            var product = new Product
            {
                ProductName = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageURL = dto.ImageURL,
                CategoryID = dto.CategoryId
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveAsync();

            dto.Id = product.ProductID; // Cập nhật lại ID sau khi lưu
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, ProductDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return false;

            // Cập nhật thông tin
            product.ProductName = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.ImageURL = dto.ImageURL;
            product.CategoryID = dto.CategoryId;

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return false;

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}