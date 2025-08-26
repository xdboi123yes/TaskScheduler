using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskScheduler.Business.Interfaces;
using TaskScheduler.Entities; // TaskStatus enum'ı için
using TaskScheduler.Web.Areas.Personnel.Models.DashboardViewModels;


namespace TaskScheduler.Web.Areas.Personnel.Controllers
{
    [Area("Personnel")]
    [Authorize(Roles = "Personnel")] // Sadece "Personnel" rolündeki kullanıcılar erişebilir
    public class DashboardController : Controller
    {
        private readonly IScheduleService _scheduleService;

        public DashboardController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Challenge();
            }
            var userId = int.Parse(userIdClaim);

            var activeSchedule = await _scheduleService.GetActiveScheduleAsync();
            
            var myTasks = activeSchedule?.ScheduledTasks
                            .Where(st => st.Personnel?.User?.Id == userId)
                            .OrderBy(st => st.DayOfWeek)
                            .ToList() ?? new List<ScheduledTask>();

            var viewModel = new MyScheduleViewModel
            {
                Tasks = myTasks,
                // YENİ KOD: Görevleri güne göre grupla
                TasksByDay = myTasks.GroupBy(t => t.DayOfWeek).OrderBy(g => g.Key).ToArray()
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int scheduledTaskId, Entities.TaskStatus newStatus)
        {
            await _scheduleService.UpdateTaskStatusAsync(scheduledTaskId, newStatus);
            return RedirectToAction(nameof(Index));
        }

    }
}