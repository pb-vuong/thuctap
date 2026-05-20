
using mobileshopping.Models;

namespace mobileshopping.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string includeProperties);
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        Task GetFirstOrDefaultAsync(Func<object, bool> value, string includeProperties);
        void Remove(Product product);
    }
}
