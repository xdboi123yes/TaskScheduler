using TaskScheduler.Business.Interfaces;
using TaskScheduler.DataAccess.Interfaces;
using TaskScheduler.Entities;
using System.Linq; // Where metodu için
using System.Threading.Tasks; // Task için

namespace TaskScheduler.Business.Services
{
    public class PersonnelService : IPersonnelService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PersonnelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Personnel>> GetAllPersonnelAsync()
        {
            var allPersonnel = await _unitOfWork.Personnel.GetAllAsync();
            return allPersonnel.Where(p => !p.IsDeleted);
        }
        
        // GetByIdAsync metodu IRepository'de var, buraya ekleyelim.
        public async Task<Personnel?> GetPersonnelByIdAsync(int id)
        {
            return await _unitOfWork.Personnel.GetByIdAsync(id);
        }

        // --- HATALI KISIMLARIN DÜZELTİLMİŞ HALİ ---

        public async System.Threading.Tasks.Task CreatePersonnelAsync(Personnel personnel)
        {
            await _unitOfWork.Personnel.AddAsync(personnel);
            await _unitOfWork.CompleteAsync();
        }

        public async System.Threading.Tasks.Task UpdatePersonnelAsync(Personnel personnel)
        {
            // Update senkron bir metot olduğu için await'e gerek yok
            _unitOfWork.Personnel.Update(personnel); 
            await _unitOfWork.CompleteAsync();
        }

        public async System.Threading.Tasks.Task DeletePersonnelAsync(int id)
        {
            var personnel = await _unitOfWork.Personnel.GetByIdAsync(id);
            if (personnel != null)
            {
                personnel.IsDeleted = true;
                _unitOfWork.Personnel.Update(personnel); // Update senkron
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}