using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskScheduler.Web.Areas.Admin.Controllers
{
    [Area("Admin")] // Bu controller'ın Admin Area'sına ait olduğunu belirtir
    [Authorize(Roles = "Admin")] // Bu controller'a sadece "Admin" rolündeki kullanıcılar erişebilir
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}