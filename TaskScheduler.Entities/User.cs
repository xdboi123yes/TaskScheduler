using System.ComponentModel.DataAnnotations.Schema;

namespace TaskScheduler.Entities
{
    public class User : BaseEntity
    {
        public required string Username { get; set; }
        public required string PasswordHash { get; set; } // Gerçek projede hash'lenmiş şifre tutulur
        public UserRole Role { get; set; }

        public int? PersonnelId { get; set; }
        public virtual Personnel? Personnel { get; set; }
    }
}