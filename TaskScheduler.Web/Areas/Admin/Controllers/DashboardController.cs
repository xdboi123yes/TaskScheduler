using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using TaskScheduler.Business.Interfaces;
using TaskScheduler.Web.Areas.Admin.Models.DashboardViewModels;

namespace TaskScheduler.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IScheduleService _scheduleService;
        private readonly IPersonnelService _personnelService;
        private readonly ITaskService _taskService;

        public DashboardController(IScheduleService scheduleService, IPersonnelService personnelService, ITaskService taskService)
        {
            _scheduleService = scheduleService;
            _personnelService = personnelService;
            _taskService = taskService;
        }

        public async Task<IActionResult> Index()
        {
            var activeSchedule = await _scheduleService.GetActiveScheduleAsync();
            var allPersonnel = (await _personnelService.GetAllPersonnelAsync()).ToList();
            var allTasks = (await _taskService.GetAllTasksAsync()).ToList();

            var viewModel = new DashboardViewModel
            {
                ActiveSchedule = activeSchedule,
                PersonnelList = allPersonnel,
                TotalPersonnelCount = allPersonnel.Count,
                TotalTaskCount = allTasks.Count
            };

            return View(viewModel);
        }
    }
}