namespace TaskScheduler.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPersonnelRepository Personnel { get; }
        ITaskRepository Task { get; }
        IUserRepository User { get; }
        IWeeklyScheduleRepository WeeklySchedule { get; }
        IScheduledTaskRepository ScheduledTask { get; }
        
        Task<int> CompleteAsync();
    }
}