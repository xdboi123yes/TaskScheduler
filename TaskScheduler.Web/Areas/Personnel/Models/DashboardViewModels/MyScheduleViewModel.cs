using System.Collections.Generic;
using System.Linq;
using TaskScheduler.Entities;

namespace TaskScheduler.Web.Areas.Personnel.Models.DashboardViewModels
{
    public class MyScheduleViewModel
    {
        public List<ScheduledTask> Tasks { get; set; } = new List<ScheduledTask>();
        
        // Günleri ve o güne ait görevleri gruplayan bir yapı
        public IGrouping<int, ScheduledTask>[]? TasksByDay { get; set; }

        public Dictionary<int, string> DaysOfWeek { get; set; } = new Dictionary<int, string>
        {
            { 1, "Pazartesi" }, { 2, "Salı" }, { 3, "Çarşamba" }, { 4, "Perşembe" }, { 5, "Cuma" }, { 6, "Cumartesi" }
        };
    }
}