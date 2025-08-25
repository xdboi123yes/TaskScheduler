using TaskScheduler.Entities;
using Task = TaskScheduler.Entities.Task;
namespace TaskScheduler.DataAccess.Interfaces
{
    public interface ITaskRepository : IRepository<Task> { }
}