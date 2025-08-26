using System.ComponentModel.DataAnnotations;
namespace TaskScheduler.Web.Areas.Admin.Models.PersonnelViewModels
{
    public class PersonnelCreateViewModel
    {
        [Required(ErrorMessage = "Personel adı zorunludur.")]
        [Display(Name = "Adı")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Personel soyadı zorunludur.")]
        [Display(Name = "Soyadı")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public required string Email { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;

        // Kullanıcı hesabı oluşturmak için yeni alanlar
        [Display(Name = "Kullanıcı Hesabı Oluşturulsun Mu?")]
        public bool CreateUserAccount { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        public string? Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string? Password { get; set; }
    }
}