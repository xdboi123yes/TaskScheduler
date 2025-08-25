using System.ComponentModel.DataAnnotations;

namespace TaskScheduler.Entities
{
    public class Task : BaseEntity
    {
        public required string Name { get; set; }

        [Range(1, 6)]
        public int DifficultyLevel { get; set; }

        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; } = new List<ScheduledTask>();
    }
}