using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskScheduler.Business.Interfaces;
using TaskScheduler.Entities;
using TaskScheduler.Web.Areas.Admin.Models.PersonnelViewModels;

namespace TaskScheduler.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PersonnelController : Controller
    {
        private readonly IPersonnelService _personnelService;

        public PersonnelController(IPersonnelService personnelService)
        {
            _personnelService = personnelService;
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
                var personnel = new Personnel
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    IsActive = model.IsActive
                };
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
                IsActive = personnel.IsActive
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
    }
}