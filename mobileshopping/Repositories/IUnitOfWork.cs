using mobileshopping.Models;
namespace mobileshopping.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<User> Users { get; }
        IGenericRepository<CartItem> CartItems { get; }
        IGenericRepository<Cart> Carts { get; }
        Task<int> SaveAsync();
    }
}
