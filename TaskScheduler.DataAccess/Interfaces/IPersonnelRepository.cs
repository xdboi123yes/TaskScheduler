using TaskScheduler.Entities;

namespace TaskScheduler.DataAccess.Interfaces
{
    public interface IPersonnelRepository : IRepository<Personnel>
    {
        // Buraya gelecekte Personnel'e özel metotlar eklenebilir.
        // Örneğin: Task<IEnumerable<Personnel>> GetActivePersonnelAsync();
    }
}