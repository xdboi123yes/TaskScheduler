namespace TaskScheduler.Entities
{
    // abstract: Bu sınıftan doğrudan nesne yaratılamaz, sadece miras alınabilir.
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false; // Soft delete için
    }
}