using Microsoft.AspNetCore.DataProtection;

namespace HumanResource.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var keyDirectory = Path.Combine(
                builder.Environment.ContentRootPath,
                "App_Data",
                "DataProtectionKeys");
            Directory.CreateDirectory(keyDirectory);

            builder.Services.AddControllersWithViews();
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keyDirectory))
                .SetApplicationName("HumanResource.MVC");
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".HumanResource.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(45);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHttpClient<Services.HrApiClient>(client =>
            {
                var baseUrl = builder.Configuration["ApiSettings:BaseUrl"]
                    ?? "http://localhost:5032/";

                client.BaseAddress = new Uri(baseUrl);
            });
            builder.Services.AddScoped<Services.ModulePageService>();

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
