using TaskScheduler.DataAccess.Data;
using TaskScheduler.DataAccess.Interfaces;
using TaskScheduler.Entities;
using Task = TaskScheduler.Entities.Task; // Entity sınıfları için using ekleyin

namespace TaskScheduler.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IPersonnelRepository Personnel { get; private set; }
        public ITaskRepository Task { get; private set; }
        public IUserRepository User { get; private set; }
        public IWeeklyScheduleRepository WeeklySchedule { get; private set; }
        public IScheduledTaskRepository ScheduledTask { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            // Repository'leri oluştur
            Personnel = new PersonnelRepository(_context);
            Task = new TaskRepository(_context);
            User = new UserRepository(_context);
            WeeklySchedule = new WeeklyScheduleRepository(_context);
            ScheduledTask = new ScheduledTaskRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
    
    // Not: Henüz somut repository sınıflarını oluşturmadık.
    // Şimdi onları oluşturalım ki yukarıdaki kod çalışsın.
    
    // SOMUT REPOSITORY SINIFLARI
    public class PersonnelRepository : Repository<Personnel>, IPersonnelRepository
    {
        public PersonnelRepository(AppDbContext context) : base(context) { }
    }

    public class TaskRepository : Repository<Task>, ITaskRepository
    {
        public TaskRepository(AppDbContext context) : base(context) { }
    }
    
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }
    }
    
    public class WeeklyScheduleRepository : Repository<WeeklySchedule>, IWeeklyScheduleRepository
    {
        public WeeklyScheduleRepository(AppDbContext context) : base(context) { }
    }

    public class ScheduledTaskRepository : Repository<ScheduledTask>, IScheduledTaskRepository
    {
        public ScheduledTaskRepository(AppDbContext context) : base(context) { }
    }
}