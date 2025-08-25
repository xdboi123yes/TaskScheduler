using TaskScheduler.Entities;
using System.Threading.Tasks; // Bu using ifadesini ekleyin!

namespace TaskScheduler.Business.Interfaces
{
    public interface IPersonnelService
    {
        Task<IEnumerable<Personnel>> GetAllPersonnelAsync();
        Task<Personnel?> GetPersonnelByIdAsync(int id);
        System.Threading.Tasks.Task CreatePersonnelAsync(Personnel personnel); // Task olmalı, Task<...> değil
        System.Threading.Tasks.Task UpdatePersonnelAsync(Personnel personnel); // Task olmalı
        System.Threading.Tasks.Task DeletePersonnelAsync(int id);             // Task olmalı
    }
}