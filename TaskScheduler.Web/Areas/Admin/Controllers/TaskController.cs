using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskScheduler.Business.Interfaces;
using TaskScheduler.Web.Areas.Admin.Models.TaskViewModels;
// Entity'yi Task olarak kullanabilmek için alias veriyoruz, çünkü System.Threading.Tasks.Task ile çakışıyor.
using Task = TaskScheduler.Entities.Task;


namespace TaskScheduler.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: Admin/Task
        public async Task<IActionResult> Index()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return View(tasks);
        }

        // GET: Admin/Task/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var task = new Task { Name = model.Name, DifficultyLevel = model.DifficultyLevel };
                await _taskService.CreateTaskAsync(task);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Admin/Task/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var model = new TaskEditViewModel { Id = task.Id, Name = task.Name, DifficultyLevel = task.DifficultyLevel };
            return View(model);
        }

        // POST: Admin/Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var taskToUpdate = await _taskService.GetTaskByIdAsync(id);
                if(taskToUpdate == null) return NotFound();

                taskToUpdate.Name = model.Name;
                taskToUpdate.DifficultyLevel = model.DifficultyLevel;
                
                await _taskService.UpdateTaskAsync(taskToUpdate);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Admin/Task/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Admin/Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _taskService.DeleteTaskAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}