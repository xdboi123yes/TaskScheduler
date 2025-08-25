namespace TaskScheduler.Entities
{
    public class WeeklySchedule : BaseEntity
    {
        public DateTime WeekStartDate { get; set; }
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; } = new List<ScheduledTask>();
    }
}