using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskScheduler.Business.Interfaces;
using TaskScheduler.DataAccess.Interfaces;
using TaskScheduler.Entities;
using Task = TaskScheduler.Entities.Task;

namespace TaskScheduler.Business.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WeeklySchedule?> GetLatestScheduleAsync()
        {
            // Veritabanından en son oluşturulan planı getir.
            // İlişkili verileri de (ScheduledTasks, Personnel, Task) Eager Loading ile çekiyoruz.
            return await _unitOfWork.WeeklySchedule
                .GetAll()
                .OrderByDescending(ws => ws.GeneratedDate)
                .Include(ws => ws.ScheduledTasks)
                    .ThenInclude(st => st.Personnel)
                .Include(ws => ws.ScheduledTasks)
                    .ThenInclude(st => st.Task)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<WeeklySchedule>> GetWeeklySchedulesWithDetailsAsync()
        {
            return await _unitOfWork.WeeklySchedule
                .GetAll()
                .Include(ws => ws.ScheduledTasks)
                    .ThenInclude(st => st.Task)
                .Include(ws => ws.ScheduledTasks)
                    .ThenInclude(st => st.Personnel)
                .OrderBy(ws => ws.WeekStartDate)
                .ThenBy(ws => ws.GeneratedDate)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task GenerateWeeklyScheduleAsync(List<int> workingDays)
        {
            // 1. Gerekli verileri veritabanından çek.
            var activePersonnel = (await _unitOfWork.Personnel.GetAllAsync()).Where(p => p.IsActive && !p.IsDeleted).ToList();
            var allTasks = (await _unitOfWork.Task.GetAllAsync()).Where(t => !t.IsDeleted).ToList();

            // Yeterli personel veya görev yoksa hata fırlat.
            if (!activePersonnel.Any() || !allTasks.Any())
            {
                throw new InvalidOperationException("Plan oluşturmak için yeterli aktif personel veya görev bulunmuyor.");
            }
            
            // 2. Yeni bir haftalık plan konteyneri oluştur.
            var newSchedule = new WeeklySchedule
            {
                // Haftanın başlangıcı olarak geçen Pazartesi'yi bulalım.
                WeekStartDate = GetStartOfWeek(DateTime.Today, DayOfWeek.Monday)
            };

            // 3. Algoritma için yardımcı veri yapıları.
            var random = new Random();
            // Her personelin bir önceki gün hangi zorlukta görev aldığını tutacak sözlük.
            var lastDifficultyPerPersonnel = new Dictionary<int, int>();
            activePersonnel.ForEach(p => lastDifficultyPerPersonnel.Add(p.Id, 0)); // Başlangıçta 0 (yok)

            // 4. DAĞITIM ALGORİTMASI
            foreach (var day in workingDays.OrderBy(d => d)) // Günleri sırayla işle (Pzt, Salı...)
            {
                // O gün atanabilecek görevlerin bir kopyasını oluştur (görev havuzu).
                var availableTasks = new List<Task>(allTasks);
                // Personel listesini her gün için karıştırarak adil bir dağıtım sağla.
                var shuffledPersonnel = activePersonnel.OrderBy(p => random.Next()).ToList();

                foreach (var personnel in shuffledPersonnel)
                {
                    int lastDifficulty = lastDifficultyPerPersonnel[personnel.Id];

                    // KURAL: Bir önceki günün zorluğuna ardışık olmayan görevleri bul.
                    var suitableTasks = availableTasks
                        .Where(t => t.DifficultyLevel != lastDifficulty - 1 && 
                                    t.DifficultyLevel != lastDifficulty + 1)
                        .ToList();
                    
                    // Eğer kurala uyan görev kalmadıysa, kuralı esnet ve havuzdaki herhangi bir görevi al.
                    // Bu, görev/personel sayısının az olduğu durumlarda sistemin kilitlenmesini önler.
                    if (!suitableTasks.Any())
                    {
                        suitableTasks = availableTasks;
                    }
                    
                    // Eğer hala atanacak görev yoksa (örn: personel > görev sayısı), bu personeli atla.
                    if (!suitableTasks.Any()) continue;

                    // Uygun görevler arasından rastgele birini seç.
                    var selectedTask = suitableTasks[random.Next(suitableTasks.Count)];

                    // Yeni görev atamasını oluştur.
                    var scheduledTask = new ScheduledTask
                    {
                        PersonnelId = personnel.Id,
                        TaskId = selectedTask.Id,
                        DayOfWeek = day,
                        WeeklySchedule = newSchedule // Bu atama, bu yeni plana ait.
                    };

                    // Oluşturulan görevi, ana planın görev listesine ekle.
                    newSchedule.ScheduledTasks.Add(scheduledTask);

                    // Bu görev artık havuzdan kaldırılmalı ki başka birine atanmasın.
                    availableTasks.Remove(selectedTask);
                    
                    // Personelin son görev zorluğunu güncelle.
                    lastDifficultyPerPersonnel[personnel.Id] = selectedTask.DifficultyLevel;
                }
            }

            // 5. Oluşturulan tüm planı veritabanına kaydet.
            await _unitOfWork.WeeklySchedule.AddAsync(newSchedule);
            await _unitOfWork.CompleteAsync();
        }

        // Haftanın başlangıç gününü bulan yardımcı metot.
        private DateTime GetStartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}