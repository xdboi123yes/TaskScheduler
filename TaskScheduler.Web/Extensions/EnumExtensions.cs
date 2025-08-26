using System.ComponentModel.DataAnnotations;
using System.Linq;
using TaskScheduler.Entities;

namespace TaskScheduler.Web.Extensions
{
    public static class EnumExtensions
    {
        public static string ToTurkishString(this ScheduleStatus status)
        {
            return status switch
            {
                ScheduleStatus.Draft => "Taslak",
                ScheduleStatus.Active => "Aktif",
                ScheduleStatus.Archived => "Arşivlenmiş",
                _ => status.ToString()
            };
        }

        public static string GetBadgeClass(this ScheduleStatus status)
        {
            return status switch
            {
                ScheduleStatus.Draft => "bg-warning text-dark",
                ScheduleStatus.Active => "bg-success",
                ScheduleStatus.Archived => "bg-secondary",
                _ => "bg-light text-dark"
            };
        }

        // Personel paneli için TaskStatus'ı da Türkçeleştirelim
        public static string ToTurkishString(this Entities.TaskStatus status)
        {
            return status switch
            {
                Entities.TaskStatus.Pending => "Bekliyor",
                Entities.TaskStatus.InProgress => "Devam Ediyor",
                Entities.TaskStatus.Completed => "Tamamlandı",
                Entities.TaskStatus.Postponed => "Ertelendi",
                _ => status.ToString()
            };
        }

        public static string GetBadgeClass(this Entities.TaskStatus status)
        {
            return status switch
            {
                Entities.TaskStatus.Completed => "bg-success",
                Entities.TaskStatus.InProgress => "bg-primary",
                Entities.TaskStatus.Postponed => "bg-secondary",
                Entities.TaskStatus.Pending => "bg-warning text-dark",
                _ => "bg-light text-dark"
            };
        }
    
    }
}