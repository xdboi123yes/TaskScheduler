using System.Collections.Generic;
using System.Threading.Tasks;
using TaskScheduler.Entities;

namespace TaskScheduler.Business.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<Entities.Task>> GetAllTasksAsync();
        Task<Entities.Task?> GetTaskByIdAsync(int id);
        System.Threading.Tasks.Task CreateTaskAsync(Entities.Task task);
        System.Threading.Tasks.Task UpdateTaskAsync(Entities.Task task);
        System.Threading.Tasks.Task DeleteTaskAsync(int id);

    }
}