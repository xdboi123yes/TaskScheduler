namespace TaskScheduler.Entities
{
    // Bu tablo, diğer tabloları birbirine bağlayan bir "junction" tablosudur.
    // Bu yüzden BaseEntity'den türemesine gerek yok.
    public class ScheduledTask
    {
        public int Id { get; set; }

        // İlişkiler için Foreign Key'ler
        public int WeeklyScheduleId { get; set; }
        public int PersonnelId { get; set; }
        public int TaskId { get; set; }

        // Navigation Property'ler
        public virtual WeeklySchedule? WeeklySchedule { get; set; }
        public virtual Personnel? Personnel { get; set; }
        public virtual Task? Task { get; set; }

        public int DayOfWeek { get; set; } // 1: Pazartesi, 2: Salı...
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
    }
}