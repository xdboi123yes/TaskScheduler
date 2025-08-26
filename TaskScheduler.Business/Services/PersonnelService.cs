using TaskScheduler.Business.Interfaces;
using TaskScheduler.DataAccess.Interfaces;
using TaskScheduler.Entities;
using Microsoft.EntityFrameworkCore; // Include metodu için
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
            var personnel = await _unitOfWork.Personnel.GetAll() // Asenkron olmayan GetAll()
                                .Include(p => p.User)
                                .FirstOrDefaultAsync(p => p.Id == id); // await en sonda!
            return personnel;
        }

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
        
        public async System.Threading.Tasks.Task AddUserToPersonnelAsync(int personnelId, User newUser)
        {
            var personnel = await _unitOfWork.Personnel.GetByIdAsync(personnelId);
            if (personnel == null)
            {
                throw new System.Exception("Personel bulunamadı.");
            }

            // Yeni kullanıcıyı personel ile ilişkilendir
            newUser.PersonnelId = personnelId;
            
            await _unitOfWork.User.AddAsync(newUser);
            await _unitOfWork.CompleteAsync();
        }

        public async System.Threading.Tasks.Task RemoveUserFromPersonnelAsync(int personnelId)
        {
            var personnel = await _unitOfWork.Personnel.GetAll()
                                .Include(p => p.User)
                                .FirstOrDefaultAsync(p => p.Id == personnelId);

            if (personnel?.User == null)
            {
                return;
            }

            _unitOfWork.User.Delete(personnel.User);
            await _unitOfWork.CompleteAsync();
        }
    }
}