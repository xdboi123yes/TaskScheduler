namespace TaskScheduler.Entities
{
    public class Personnel : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Bir personele ait kullanıcı hesabı
        public virtual User? User { get; set; }

        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; } = new List<ScheduledTask>();
    }
}