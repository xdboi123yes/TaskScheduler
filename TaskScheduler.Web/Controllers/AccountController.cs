using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskScheduler.DataAccess.Interfaces;
using TaskScheduler.Entities;
using TaskScheduler.Web.ViewModels.Account;

namespace TaskScheduler.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var users = await _unitOfWork.User.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Username.Equals(model.Username, StringComparison.OrdinalIgnoreCase));


            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                return View(model);
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                return View(model);
            }

            // Kullanıcı bilgileri doğru, şimdi cookie oluşturalım.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // "Beni hatırla" gibi
            };

            await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

            // Kullanıcıyı rolüne göre doğru panele yönlendir
            if (user.Role == UserRole.Admin)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            else // if (user.Role == UserRole.Personnel)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Personnel" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous] // Herkesin erişebilmesi için
        public async Task<IActionResult> InitialAdminSetup()
        {
            var users = await _unitOfWork.User.GetAllAsync();
            if (users.Any())
            {
                // Sistemde zaten kullanıcı varsa, bu sayfaya erişimi engelle ve login'e yönlendir.
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> InitialAdminSetup(InitialAdminSetupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var users = await _unitOfWork.User.GetAllAsync();
            if (users.Any())
            {
                // Form gönderilirken arada başka biri admin oluşturmuş olabilir.
                return RedirectToAction("Login");
            }

            var adminUser = new User
            {
                Username = model.Username,
                Role = UserRole.Admin,
                PasswordHash = "" // Initialize PasswordHash to satisfy the required member constraint
            };
            adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, model.Password);

            await _unitOfWork.User.AddAsync(adminUser);
            await _unitOfWork.CompleteAsync();

            // Admin oluşturulduktan sonra direkt login sayfasına yönlendir.
            return RedirectToAction("Login");
        }
    
    }
}