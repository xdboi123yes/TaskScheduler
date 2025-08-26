using Microsoft.EntityFrameworkCore;
using TaskScheduler.DataAccess.Interfaces;

namespace TaskScheduler.Web.Middlewares
{
    public class AdminCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
        {
            // İzin verilen yollar (bu yollara erişimde admin kontrolü yapılmayacak)
            var allowedPaths = new[] { "/Account/InitialAdminSetup", "/lib", "/css", "/js" };
            var requestPath = context.Request.Path;
            
            // DÜZELTME: requestPath.Value null olabileceğinden, önce HasValue ile kontrol ediyoruz.
            if (requestPath.HasValue && allowedPaths.Any(p => requestPath.Value.StartsWith(p)))
            {
                await _next(context);
                return;
            }
            
            // PERFORMANS İYİLEŞTİRMESİ: Tüm kullanıcıları çekmek yerine, veritabanında var olup olmadığını sor.
            if (!await unitOfWork.User.GetAll().AnyAsync())
            {
                // Kullanıcı yoksa ve istek InitialAdminSetup sayfasına değilse, oraya yönlendir.
                if (requestPath != "/Account/InitialAdminSetup")
                {
                    context.Response.Redirect("/Account/InitialAdminSetup");
                    return;
                }
            }

            // Kullanıcı varsa veya zaten doğru sayfadaysa, pipeline'da devam et.
            await _next(context);
        }
    }
}   