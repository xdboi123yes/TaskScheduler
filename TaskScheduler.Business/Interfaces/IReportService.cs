using System.Threading.Tasks;
using TaskScheduler.Entities;

namespace TaskScheduler.Business.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateScheduleExcelAsync(int scheduleId);
        Task<byte[]> GenerateSchedulePdfAsync(int scheduleId);
    }
}