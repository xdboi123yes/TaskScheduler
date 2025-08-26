using TaskScheduler.Entities;

namespace TaskScheduler.Business.Interfaces
{
    public interface IScheduleService
    {
        // Artık oluşturulan taslağı geri döndürecek.
        Task<WeeklySchedule> GenerateDraftScheduleAsync(List<int> workingDays);

        // Yeni metotlar
        Task<WeeklySchedule?> GetActiveScheduleAsync();
        Task<IEnumerable<WeeklySchedule>> GetAllSchedulesAsync();
        System.Threading.Tasks.Task ApproveScheduleAsync(int scheduleId);
        System.Threading.Tasks.Task ArchiveScheduleAsync(int scheduleId);
        System.Threading.Tasks.Task DeleteScheduleAsync(int scheduleId);
        Task<WeeklySchedule?> GetScheduleDetailsByIdAsync(int scheduleId);
        System.Threading.Tasks.Task UpdateTaskStatusAsync(int scheduledTaskId, Entities.TaskStatus newStatus);
    }
}