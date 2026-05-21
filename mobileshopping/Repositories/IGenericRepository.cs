
using mobileshopping.Models;
using System.Linq.Expressions;

namespace mobileshopping.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null);
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, string? includeProperties = null);
    }
}
