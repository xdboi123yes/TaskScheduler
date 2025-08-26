using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskScheduler.Business.Interfaces;
using TaskScheduler.DataAccess.Interfaces;
using TaskScheduler.Entities;
// Alias kullanarak System.Threading.Tasks.Task ile çakışmasını önlüyoruz.
using Task = TaskScheduler.Entities.Task; 

namespace TaskScheduler.Business.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Random _random = new Random();

        public ScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // --- IScheduleService Implementasyonları ---

        public async Task<WeeklySchedule> GenerateDraftScheduleAsync(List<int> workingDays)
        {
            var activePersonnel = (await _unitOfWork.Personnel.GetAllAsync()).Where(p => p.IsActive && !p.IsDeleted).ToList();
            var allTasks = (await _unitOfWork.Task.GetAllAsync()).Where(t => !t.IsDeleted).ToList();

            if (activePersonnel.Count == 0 || allTasks.Count == 0)
                throw new InvalidOperationException("Plan oluşturmak için yeterli aktif personel veya görev bulunmuyor.");

            var weekStartDate = GetStartOfWeek(DateTime.Today, DayOfWeek.Monday);
            var newSchedule = new WeeklySchedule
            {
                WeekStartDate = weekStartDate,
                ScheduleName = $"{weekStartDate:yyyy-MM-dd} Taslağı",
                Status = ScheduleStatus.Draft
            };
            
            var taskQueues = new Dictionary<int, Queue<Task>>();
            foreach (var p in activePersonnel)
            {
                var shuffledTasks = allTasks.OrderBy(t => t.DifficultyLevel).ThenBy(t => _random.Next()).ToList();
                taskQueues[p.Id] = new Queue<Task>(shuffledTasks);
            }

            var lastDifficultyPerPersonnel = new Dictionary<int, int>();
            activePersonnel.ForEach(p => lastDifficultyPerPersonnel[p.Id] = 0);

            foreach (var day in workingDays.OrderBy(d => d))
            {
                var personnelForDay = activePersonnel.OrderBy(p => _random.Next()).ToList();
                var assignedTasksToday = new HashSet<int>();

                foreach (var personnel in personnelForDay)
                {
                    int lastDifficulty = lastDifficultyPerPersonnel[personnel.Id];
                    var personnelTaskQueue = taskQueues[personnel.Id];
                    
                    Task? selectedTask = null;
                    int attempts = 0;
                    int maxAttempts = personnelTaskQueue.Count;

                    while (attempts < maxAttempts)
                    {
                        var candidateTask = personnelTaskQueue.Dequeue();
                        attempts++;

                        if (Math.Abs(candidateTask.DifficultyLevel - lastDifficulty) > 1 && !assignedTasksToday.Contains(candidateTask.Id))
                        {
                            selectedTask = candidateTask;
                            // Kullanılmayan görevleri kuyruğun sonuna geri ekle
                            for (int i = 0; i < attempts - 1; i++)
                            {
                                personnelTaskQueue.Enqueue(personnelTaskQueue.Dequeue());
                            }
                            break;
                        }
                        else
                        {
                            personnelTaskQueue.Enqueue(candidateTask);
                        }
                    }

                    if (selectedTask != null)
                    {
                        var scheduledTask = new ScheduledTask
                        {
                            PersonnelId = personnel.Id,
                            TaskId = selectedTask.Id,
                            DayOfWeek = day,
                            // WeeklyScheduleId EF Core tarafından otomatik atanacak
                        };
                        newSchedule.ScheduledTasks.Add(scheduledTask);
                        assignedTasksToday.Add(selectedTask.Id);
                        lastDifficultyPerPersonnel[personnel.Id] = selectedTask.DifficultyLevel;
                    }
                }
            }

            await _unitOfWork.WeeklySchedule.AddAsync(newSchedule);
            await _unitOfWork.CompleteAsync();
            return newSchedule;
        }

        public async Task<WeeklySchedule?> GetActiveScheduleAsync()
        {
            return await _unitOfWork.WeeklySchedule.GetAll()
                .Include(ws => ws.ScheduledTasks)
                    .ThenInclude(st => st.Task) // Görev detayları için
                .Include(ws => ws.ScheduledTasks)
                    .ThenInclude(st => st.Personnel)
                        .ThenInclude(p => p!.User) // <-- EKSİK OLAN KRİTİK SATIR
                .FirstOrDefaultAsync(ws => ws.Status == ScheduleStatus.Active);
        }


        public async Task<IEnumerable<WeeklySchedule>> GetAllSchedulesAsync()
        {
            return await _unitOfWork.WeeklySchedule.GetAll()
                .OrderByDescending(ws => ws.WeekStartDate)
                .ThenBy(ws => ws.Status)
                .ToListAsync();
        }
        
        public async System.Threading.Tasks.Task UpdateTaskStatusAsync(int scheduledTaskId, Entities.TaskStatus newStatus)
        {
            var scheduledTask = await _unitOfWork.ScheduledTask.GetByIdAsync(scheduledTaskId);
            if (scheduledTask != null)
            {
                scheduledTask.Status = newStatus;
                _unitOfWork.ScheduledTask.Update(scheduledTask);
                await _unitOfWork.CompleteAsync();
            }
        }
        
        public async System.Threading.Tasks.Task ApproveScheduleAsync(int scheduleId)
        {
            var scheduleToApprove = await _unitOfWork.WeeklySchedule.GetByIdAsync(scheduleId);
            if (scheduleToApprove == null) return;

            var existingActive = await _unitOfWork.WeeklySchedule.GetAll()
                .FirstOrDefaultAsync(ws => ws.WeekStartDate == scheduleToApprove.WeekStartDate
                                         && ws.Status == ScheduleStatus.Active
                                         && ws.Id != scheduleToApprove.Id);

            if (existingActive != null)
            {
                existingActive.Status = ScheduleStatus.Archived;
                existingActive.ScheduleName = $"{existingActive.WeekStartDate:yyyy-MM-dd} Arşivlenmiş Plan";
                _unitOfWork.WeeklySchedule.Update(existingActive);
            }

            scheduleToApprove.Status = ScheduleStatus.Active;
            scheduleToApprove.ScheduleName = $"{scheduleToApprove.WeekStartDate:yyyy-MM-dd} Aktif Planı";
            _unitOfWork.WeeklySchedule.Update(scheduleToApprove);
            await _unitOfWork.CompleteAsync();
        }
        
        public async System.Threading.Tasks.Task ArchiveScheduleAsync(int scheduleId)
        {
            var scheduleToArchive = await _unitOfWork.WeeklySchedule.GetByIdAsync(scheduleId);
            if (scheduleToArchive != null)
            {
                scheduleToArchive.Status = ScheduleStatus.Archived;
                scheduleToArchive.ScheduleName = $"{scheduleToArchive.WeekStartDate:yyyy-MM-dd} Arşivlenmiş Plan";
                _unitOfWork.WeeklySchedule.Update(scheduleToArchive);
                await _unitOfWork.CompleteAsync();
            }
        }
            
        public async System.Threading.Tasks.Task DeleteScheduleAsync(int scheduleId)
        {
            var scheduleToDelete = await _unitOfWork.WeeklySchedule.GetByIdAsync(scheduleId);
            if (scheduleToDelete != null)
            {
                _unitOfWork.WeeklySchedule.Delete(scheduleToDelete);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<WeeklySchedule?> GetScheduleDetailsByIdAsync(int scheduleId)
        {
            return await _unitOfWork.WeeklySchedule.GetAll()
                .Include(ws => ws.ScheduledTasks).ThenInclude(st => st.Personnel)
                .Include(ws => ws.ScheduledTasks).ThenInclude(st => st.Task)
                .FirstOrDefaultAsync(ws => ws.Id == scheduleId);
        }

        public IQueryable<WeeklySchedule> GetAllSchedulesAsQueryable()
        {
            return _unitOfWork.WeeklySchedule.GetAll()
                .OrderByDescending(ws => ws.WeekStartDate)
                .ThenBy(ws => ws.Status);
        }

        // --- Yardımcı Metot ---

        private DateTime GetStartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}