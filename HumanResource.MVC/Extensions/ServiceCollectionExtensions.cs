using HumanResource.MVC.Models.Config;
using HumanResource.MVC.Services;

namespace HumanResource.MVC.Extensions;


// Extension methods that keep Program.cs clean by grouping related
// service registrations in one place.

public static class ServiceCollectionExtensions
{
    
    // Registers the typed API client, resource catalog, and binds ApiSettings.
  
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind strongly-typed config so any service can inject IOptions<ApiSettings>
        services.Configure<ApiSettings>(
            configuration.GetSection(ApiSettings.SectionName));

        var baseUrl = configuration[$"{ApiSettings.SectionName}:BaseUrl"]
            ?? "http://localhost:5032/";

        services.AddHttpClient<HrApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);     // explicit timeout
        });

        services.AddSingleton<ResourceCatalog>();

        return services;
    }


    // Configures the session cookie with security best practices.
  
    public static IServiceCollection AddSessionSupport(
        this IServiceCollection services)
    {
        services.AddSession(options =>
        {
            options.Cookie.Name       = ".HumanResource.Session";
            options.IdleTimeout       = TimeSpan.FromMinutes(45);
            options.Cookie.HttpOnly   = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite   = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        });

        return services;
    }
}
