using System.Collections.Generic;
using System.Linq;
using TaskScheduler.Entities;

namespace TaskScheduler.Web.Areas.Admin.Models.DashboardViewModels
{
    public class DashboardViewModel
    {
        public WeeklySchedule? ActiveSchedule { get; set; }
        public List<TaskScheduler.Entities.Personnel> PersonnelList { get; set; } = new List<TaskScheduler.Entities.Personnel>();
        public Dictionary<int, string> DaysOfWeek { get; set; } = new Dictionary<int, string>
        {
            { 1, "Pazartesi" }, { 2, "Salı" }, { 3, "Çarşamba" }, { 4, "Perşembe" }, { 5, "Cuma" }, { 6, "Cumartesi" }
        };

        // İstatistikler için ek alanlar (ileride kullanılabilir)
        public int TotalPersonnelCount { get; set; }
        public int TotalTaskCount { get; set; }
        public int CompletedTasksToday { get; set; }
    }
}