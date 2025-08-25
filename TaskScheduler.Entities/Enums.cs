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
}