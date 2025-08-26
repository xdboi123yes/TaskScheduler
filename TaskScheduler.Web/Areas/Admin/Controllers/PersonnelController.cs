using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskScheduler.Business.Interfaces;
using TaskScheduler.Entities;
using TaskScheduler.Web.Areas.Admin.Models.PersonnelViewModels;
using Microsoft.AspNetCore.Identity;

namespace TaskScheduler.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PersonnelController : Controller
    {
        private readonly IPersonnelService _personnelService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public PersonnelController(IPersonnelService personnelService)
        {
            _personnelService = personnelService;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: /Admin/Personnel
        public async Task<IActionResult> Index()
        {
            var personnelList = await _personnelService.GetAllPersonnelAsync();
            return View(personnelList);
        }

        // GET: /Admin/Personnel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Personnel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PersonnelCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var personnel = new Entities.Personnel
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    IsActive = model.IsActive
                };
                if (model.CreateUserAccount)
                {
                    // Ekstra validasyonlar
                    if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                    {
                        ModelState.AddModelError("", "Kullanıcı hesabı oluşturmak için kullanıcı adı ve şifre zorunludur.");
                        return View(model);
                    }

                    var user = new User
                    {
                        Username = model.Username,
                        PasswordHash = "",
                        Role = UserRole.Personnel,
                        Personnel = personnel // İlişkiyi burada kuruyoruz
                    };
                    user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                    personnel.User = user;
                }
                await _personnelService.CreatePersonnelAsync(personnel);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Admin/Personnel/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var personnel = await _personnelService.GetPersonnelByIdAsync(id);
            if (personnel == null)
            {
                return NotFound();
            }

            var model = new PersonnelEditViewModel
            {
                Id = personnel.Id,
                FirstName = personnel.FirstName,
                LastName = personnel.LastName,
                Email = personnel.Email,
                IsActive = personnel.IsActive,
                HasUserAccount = personnel.User != null,
                CurrentUsername = personnel.User?.Username
            };
            return View(model);
        }

        // POST: /Admin/Personnel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PersonnelEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var personnel = await _personnelService.GetPersonnelByIdAsync(id);
                if (personnel == null)
                {
                    return NotFound();
                }

                personnel.FirstName = model.FirstName;
                personnel.LastName = model.LastName;
                personnel.Email = model.Email;
                personnel.IsActive = model.IsActive;

                await _personnelService.UpdatePersonnelAsync(personnel);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: /Admin/Personnel/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var personnel = await _personnelService.GetPersonnelByIdAsync(id);
            if (personnel == null)
            {
                return NotFound();
            }
            return View(personnel);
        }

        // POST: /Admin/Personnel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _personnelService.DeletePersonnelAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUserAccount(int id, PersonnelEditViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewUsername) || string.IsNullOrWhiteSpace(model.NewPassword))
            {
                TempData["ErrorMessage"] = "Yeni kullanıcı adı ve şifre alanları boş olamaz.";
                return RedirectToAction("Edit", new { id });
            }

            var newUser = new User
            {
                Username = model.NewUsername,
                PasswordHash = "",
                Role = UserRole.Personnel
            };
            newUser.PasswordHash = _passwordHasher.HashPassword(newUser, model.NewPassword);

            try
            {
                await _personnelService.AddUserToPersonnelAsync(id, newUser);
                TempData["SuccessMessage"] = "Kullanıcı hesabı başarıyla oluşturuldu.";
            }
            catch (System.Exception ex)
            {
                // Kullanıcı adı zaten mevcut gibi hatalar için...
                TempData["ErrorMessage"] = "Kullanıcı hesabı oluşturulurken bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("Edit", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUserAccount(int id)
        {
            try
            {
                await _personnelService.RemoveUserFromPersonnelAsync(id);
                TempData["SuccessMessage"] = "Kullanıcı hesabı başarıyla silindi.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = "Kullanıcı hesabı silinirken bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("Edit", new { id });
        }
    
    }
}