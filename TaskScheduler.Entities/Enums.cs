namespace TaskScheduler.Entities
{
    public enum UserRole
    {
        Admin = 1,
        Personnel = 2
    }

    public enum TaskStatus
    {
        Pending = 1,      // Bekliyor
        InProgress = 2,   // Devam Ediyor
        Completed = 3,    // Tamamlandı
        Postponed = 4     // Ertelendi
    }

    public enum ScheduleStatus
{
    Draft = 1,      // Taslak
    Active = 2,     // Aktif Plan
    Archived = 3    // Arşivlenmiş
}
}