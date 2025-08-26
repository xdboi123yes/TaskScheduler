using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskScheduler.Business.Interfaces;
using TaskScheduler.Entities;
using TaskScheduler.Web.Areas.Admin.Models.ScheduleViewModels;

namespace TaskScheduler.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ScheduleController : Controller
    {
        private readonly IScheduleService _scheduleService;
        private readonly IPersonnelService _personnelService;
        private readonly IReportService _reportService;

        public ScheduleController(IScheduleService scheduleService, IPersonnelService personnelService, IReportService reportService)
        {
            _scheduleService = scheduleService;
            _personnelService = personnelService;
            _reportService = reportService;
        }

        // GET: Admin/Schedule
        public async Task<IActionResult> Index()
        {
            var allSchedules = await _scheduleService.GetAllSchedulesAsync();
            return View(allSchedules);
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
                var newDraft = await _scheduleService.GenerateDraftScheduleAsync(workingDays);
                TempData["SuccessMessage"] = "Yeni taslak plan başarıyla oluşturuldu.";
                // Kullanıcıyı oluşturulan taslağın detay sayfasına yönlendir
                return RedirectToAction("Details", new { id = newDraft.Id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadPdf(int scheduleId)
        {
            var fileBytes = await _reportService.GenerateSchedulePdfAsync(scheduleId);
            if (fileBytes.Length == 0) return NotFound();

            return File(fileBytes, "application/pdf", $"HaftalikPlan_{scheduleId}.pdf");
        }

        public async Task<IActionResult> Details(int id)
        {
            var schedule = await _scheduleService.GetScheduleDetailsByIdAsync(id);

            if (schedule == null)
            {
                return NotFound();
            }

            return View("PlanDetails", schedule);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            await _scheduleService.ApproveScheduleAsync(id);
            TempData["SuccessMessage"] = "Plan başarıyla onaylandı ve aktif hale getirildi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _scheduleService.DeleteScheduleAsync(id);
            TempData["SuccessMessage"] = "Taslak plan başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reshuffle(int id)
        {
            // 1. Mevcut taslağın bilgilerini al
            var existingDraft = await _scheduleService.GetScheduleDetailsByIdAsync(id);
            if (existingDraft == null || existingDraft.Status != ScheduleStatus.Draft)
            {
                TempData["ErrorMessage"] = "Yeniden karıştırma işlemi için geçerli bir taslak bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Hangi günlerde çalışıldığını çıkar
            var workingDays = existingDraft.ScheduledTasks
                .Select(st => st.DayOfWeek)
                .Distinct()
                .ToList();

            // 3. Eski taslağı sil
            await _scheduleService.DeleteScheduleAsync(id);

            // 4. Aynı günler için yeni bir taslak oluştur
            try
            {
                var newDraft = await _scheduleService.GenerateDraftScheduleAsync(workingDays);
                TempData["SuccessMessage"] = "Taslak başarıyla yeniden karıştırıldı.";
                return RedirectToAction("Details", new { id = newDraft.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Taslak yeniden oluşturulurken bir hata oluştu: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    

    }
}