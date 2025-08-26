using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TaskScheduler.DataAccess.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> GetAll();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}