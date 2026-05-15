using System.Diagnostics;
using System.Text.Json;
using HumanResource.MVC.Models;
using HumanResource.MVC.Models.Auth;
using HumanResource.MVC.Models.Dashboard;
using HumanResource.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.MVC.Controllers;

public class HomeController : MvcControllerBase
{
    private readonly HrApiClient _apiClient;
    private readonly ILogger<HomeController> _logger;

    public HomeController(HrApiClient apiClient, ILogger<HomeController> logger)
    {
        _apiClient = apiClient;
        _logger    = logger;
    }

    public async Task<IActionResult> Index()
    {
        var login = RequireLogin();
        if (login is not null) return login;

        var model = new DashboardViewModel { Role = Role };

        if (RoleIs("Admin", "HR"))
            model.EmployeeCount = await CountFrom("api/Employees", model.Warnings);

        if (RoleIs("Admin", "HR", "Employee"))
        {
            model.DepartmentCount = await CountFrom("api/Departments", model.Warnings);
            model.JobCount        = await CountFrom("api/Jobs",        model.Warnings);
            model.LocationCount   = await CountFrom("api/Locations",   model.Warnings);
        }

        if (RoleIs("Manager"))
        {
            model.TeamMembers   = await RowsFrom("api/Employees/my-team", model.Warnings);
            model.EmployeeCount = model.TeamMembers.Count;
        }

        return View(model);
    }

    public IActionResult Profile()
    {
        var login = RequireLogin();
        if (login is not null) return login;

        return View(new ProfileViewModel { Email = Email, Role = Role, EmployeeId = EmployeeId });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    private async Task<int> CountFrom(string path, List<string> warnings)
        => (await RowsFrom(path, warnings)).Count;

    private async Task<IReadOnlyList<JsonElement>> RowsFrom(string path, List<string> warnings)
    {
        var result = await _apiClient.GetAsync(path, Token);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Dashboard failed to fetch {Path}: {Error}", path, result.ErrorMessage);
            warnings.Add(result.ErrorMessage ?? $"{path} is unavailable.");
            return [];
        }

        return ExtractRows(result.Data);
    }
}
