using System.Linq.Expressions;

namespace TaskScheduler.DataAccess.Interfaces
{
    // T, BaseEntity'den türeyen herhangi bir class olabilir.
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}