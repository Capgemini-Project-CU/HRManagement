using HumanResource.MVC.Extensions;
using Microsoft.AspNetCore.DataProtection;

namespace HumanResource.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── Logging ────────────────────────────────────────────────────────
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // ── Data protection (persists encryption keys across restarts) ─────
            var keyDirectory = Path.Combine(
                builder.Environment.ContentRootPath,
                "App_Data",
                "DataProtectionKeys");
            Directory.CreateDirectory(keyDirectory);

            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keyDirectory))
                .SetApplicationName("HumanResource.MVC");

            // ── MVC & infrastructure ───────────────────────────────────────────
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            // ── Application services (API client, catalog, typed config) ───────
            builder.Services.AddApplicationServices(builder.Configuration);

            // ── Session ────────────────────────────────────────────────────────
            builder.Services.AddSessionSupport();

            // ── Build & middleware pipeline ────────────────────────────────────
            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
