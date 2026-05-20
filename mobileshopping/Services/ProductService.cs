using mobileshopping.DTOs;
using mobileshopping.Models;
using mobileshopping.Repositories;

namespace mobileshopping.Services
{
   
   public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;

        public ProductService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            // Lấy danh sách sản phẩm kèm theo thông tin Category
            var products = await _uow.Products.GetAllAsync(includeProperties: "Category");

            // Map Entity -> DTO
            return products.Select(p => new ProductDto
            {
                Id = p.ProductID,
                Name = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryID,
                CategoryName = p.Category?.CategoryName
            });
        }

        public async Task<ProductDto> GetByIdAsync(int ProductId)
        {
            var product = await _uow.Products.GetFirstOrDefaultAsync(p => p.ProductId == ProductId, includeProperties: "Category");
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name
            };
        }

        public async Task<ProductDto> CreateAsync(ProductDto dto)
        {
            // Map DTO -> Entity
            var product = new Product
            {
                ProductName = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryID = dto.CategoryId
            };

            await _uow.Products.AddAsync(product);
            await _uow.SaveAsync();

            dto.Id = product.CategoryID; // Gán lại ID sau khi lưu db
            return dto;
        }

        public async Task UpdateAsync(int id, ProductDto dto)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product != null)
            {
                product.ProductName = dto.Name;
                product.Description = dto.Description;
                product.Price = dto.Price;
                product.CategoryID = dto.CategoryId;

                _uow.Products.Update(product);
                await _uow.SaveAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _uow.Products.GetByIdAsync(id);
            if (product != null)
            {
                _uow.Products.Remove(product);
                await _uow.SaveAsync();
            }
        }

        Task IProductService.CreateAsync(ProductDto dto)
        {
            return CreateAsync(dto);
        }
    }
}
