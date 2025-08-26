using System.ComponentModel.DataAnnotations;

namespace TaskScheduler.Web.ViewModels.Account
{
    public class InitialAdminSetupViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [Display(Name = "Admin Kullanıcı Adı")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [Display(Name = "Şifre")]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public required string ConfirmPassword { get; set; }
    }
}