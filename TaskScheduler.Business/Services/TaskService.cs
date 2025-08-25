using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskScheduler.Business.Interfaces;
using TaskScheduler.DataAccess.Interfaces;
using TaskScheduler.Entities;

namespace TaskScheduler.Business.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Entities.Task>> GetAllTasksAsync()
        {
            var allTasks = await _unitOfWork.Task.GetAllAsync();
            return allTasks.Where(t => !t.IsDeleted);
        }

        public async Task<Entities.Task?> GetTaskByIdAsync(int id)
        {
            return await _unitOfWork.Task.GetByIdAsync(id);
        }

        public async System.Threading.Tasks.Task CreateTaskAsync(Entities.Task task)
        {
            await _unitOfWork.Task.AddAsync(task);
            await _unitOfWork.CompleteAsync();
        }

        public async System.Threading.Tasks.Task UpdateTaskAsync(Entities.Task task)
        {
            _unitOfWork.Task.Update(task);
            await _unitOfWork.CompleteAsync();
        }

        public async System.Threading.Tasks.Task DeleteTaskAsync(int id)
        {
            var task = await _unitOfWork.Task.GetByIdAsync(id);
            if (task != null)
            {
                task.IsDeleted = true; // Soft delete
                _unitOfWork.Task.Update(task);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}