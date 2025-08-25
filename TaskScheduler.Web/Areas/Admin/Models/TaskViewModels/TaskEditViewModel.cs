using System.ComponentModel.DataAnnotations;

namespace TaskScheduler.Web.Areas.Admin.Models.TaskViewModels
{
    public class TaskEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Görev adı zorunludur.")]
        [Display(Name = "Görev Adı")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Zorluk seviyesi zorunludur.")]
        [Range(1, 6, ErrorMessage = "Zorluk seviyesi 1 ile 6 arasında olmalıdır.")]
        [Display(Name = "Zorluk Seviyesi")]
        public int DifficultyLevel { get; set; }
    }
}