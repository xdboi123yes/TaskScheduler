using System.Collections.Generic;
using TaskScheduler.Entities;

namespace TaskScheduler.Web.Areas.Admin.Models.ScheduleViewModels
{
    public class ScheduleIndexViewModel
    {
        public WeeklySchedule? CurrentSchedule { get; set; }
        public List<Personnel> PersonnelList { get; set; } = new List<Personnel>();
        public Dictionary<int, string> DaysOfWeek { get; set; } = new Dictionary<int, string>
        {
            { 1, "Pazartesi" }, { 2, "Salı" }, { 3, "Çarşamba" }, { 4, "Perşembe" }, { 5, "Cuma" }, { 6, "Cumartesi" }
        };

        // Plan oluşturma formu için
        public ScheduleGenerateViewModel GenerateViewModel { get; set; } = new ScheduleGenerateViewModel();
    }
    
    public class ScheduleGenerateViewModel
    {
        // Checkbox'lar için model
        public bool Monday { get; set; } = true;
        public bool Tuesday { get; set; } = true;
        public bool Wednesday { get; set; } = true;
        public bool Thursday { get; set; } = true;
        public bool Friday { get; set; } = true;
        public bool Saturday { get; set; }
    }
}