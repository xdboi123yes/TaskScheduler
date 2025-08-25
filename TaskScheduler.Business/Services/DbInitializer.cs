using Microsoft.AspNetCore.Identity; // Şifre hash'leme için bu paketi ekleyeceğiz.
using TaskScheduler.DataAccess.Interfaces;
using TaskScheduler.Entities;
using Task = TaskScheduler.Entities.Task;

namespace TaskScheduler.Business.Services
{
    public class DbInitializer
    {
        private readonly IUnitOfWork _unitOfWork;

        // Şifreleme için .NET Identity'nin şifre hash'leyicisini kullanacağız.
        // Bu daha güvenli bir yaklaşımdır.
        private readonly IPasswordHasher<User> _passwordHasher;

        public DbInitializer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async System.Threading.Tasks.Task SeedAsync()
        {
            // Veritabanında hiç kullanıcı yoksa, admin kullanıcısını oluştur.
            var users = await _unitOfWork.User.GetAllAsync();
            if (!users.Any())
            {
                var adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = "", // Initialize PasswordHash to satisfy the required member constraint
                    Role = UserRole.Admin,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow
                };
                
                // Şifreyi hash'le
                adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, "Admin123");

                await _unitOfWork.User.AddAsync(adminUser);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}