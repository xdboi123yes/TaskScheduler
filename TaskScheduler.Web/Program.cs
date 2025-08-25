using Microsoft.EntityFrameworkCore;
using TaskScheduler.DataAccess.Data; // Bunu ekleyin

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// --> BAŞLANGIÇ: Bizim ekleyeceğimiz kod
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
// --> BİTİŞ: Bizim ekleyeceğimiz kod

builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();