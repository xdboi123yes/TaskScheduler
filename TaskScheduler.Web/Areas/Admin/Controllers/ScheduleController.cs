using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskScheduler.Business.Interfaces;
using TaskScheduler.Web.Areas.Admin.Models.ScheduleViewModels;

namespace TaskScheduler.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ScheduleController : Controller
    {
        private readonly IScheduleService _scheduleService;
        private readonly IPersonnelService _personnelService;

        public ScheduleController(IScheduleService scheduleService, IPersonnelService personnelService)
        {
            _scheduleService = scheduleService;
            _personnelService = personnelService;
        }

        // GET: Admin/Schedule
        public async Task<IActionResult> Index()
        {
            var latestSchedule = await _scheduleService.GetLatestScheduleAsync();
            var allPersonnel = await _personnelService.GetAllPersonnelAsync();

            var viewModel = new ScheduleIndexViewModel
            {
                CurrentSchedule = latestSchedule,
                PersonnelList = allPersonnel.ToList()
            };

            return View(viewModel);
        }

        // POST: Admin/Schedule/Generate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(ScheduleGenerateViewModel model)
        {
            var workingDays = new List<int>();
            if (model.Monday) workingDays.Add(1);
            if (model.Tuesday) workingDays.Add(2);
            if (model.Wednesday) workingDays.Add(3);
            if (model.Thursday) workingDays.Add(4);
            if (model.Friday) workingDays.Add(5);
            if (model.Saturday) workingDays.Add(6);

            if (!workingDays.Any())
            {
                // Hata mesajı iletmek için TempData kullanabiliriz.
                TempData["ErrorMessage"] = "Lütfen en az bir çalışma günü seçin.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _scheduleService.GenerateWeeklyScheduleAsync(workingDays);
                TempData["SuccessMessage"] = "Yeni haftalık plan başarıyla oluşturuldu.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}