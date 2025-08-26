using System.Collections.Generic;
using System.Threading.Tasks;
using TaskScheduler.Entities;

namespace TaskScheduler.Business.Interfaces
{
    public interface IScheduleService
    {
        // Haftalık planı oluşturacak ana metot.
        System.Threading.Tasks.Task GenerateWeeklyScheduleAsync(List<int> workingDays);
        
        // En son oluşturulan haftalık planı getirecek metot.
        System.Threading.Tasks.Task<WeeklySchedule?> GetLatestScheduleAsync();
    }
}