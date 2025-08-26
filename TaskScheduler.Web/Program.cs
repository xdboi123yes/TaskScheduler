using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskScheduler.Business.Interfaces;
using TaskScheduler.Business.Services;
using TaskScheduler.DataAccess.Data;
using TaskScheduler.DataAccess.Interfaces;
using TaskScheduler.DataAccess.Repositories;
using TaskScheduler.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPersonnelService, PersonnelService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();


builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "TaskScheduler.AuthCookie";
        options.LoginPath = "/Account/Login";      // Kullanıcı giriş yapmamışsa bu adrese yönlendir
        options.AccessDeniedPath = "/Account/AccessDenied"; // Yetkisi yoksa bu adrese yönlendir
        options.ExpireTimeSpan = TimeSpan.FromDays(30); // Cookie'nin geçerlilik süresi
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var unitOfWork = services.GetRequiredService<IUnitOfWork>();
        var initializer = new TaskScheduler.Business.Services.DbInitializer(unitOfWork);
        await initializer.SeedAsync();
    }
    catch (Exception ex)
    {
        // Hata durumunda loglama yapılabilir.
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during DB seeding.");
    }
}

app.Run();