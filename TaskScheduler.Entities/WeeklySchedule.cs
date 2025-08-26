namespace TaskScheduler.Entities
{
    public class WeeklySchedule : BaseEntity
    {
        public DateTime WeekStartDate { get; set; }
        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; } = new List<ScheduledTask>();

        public ScheduleStatus Status { get; set; } = ScheduleStatus.Draft; // Yeni alan
        public string? ScheduleName { get; set; } // Örn: "2024 - Hafta 42 Taslağı"
    }
}