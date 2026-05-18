using System.Diagnostics;
using System.Text.Json;
using HumanResource.MVC.Models;
using HumanResource.MVC.Models.Auth;
using HumanResource.MVC.Models.Dashboard;
using HumanResource.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.MVC.Controllers;

public class HomeController : Controller
{
    private readonly HrApiClient _apiClient;

    private string? Token => HttpContext.Session.GetString("JwtToken");

    private string Role => HttpContext.Session.GetString("UserRole") ?? string.Empty;

    private string Email => HttpContext.Session.GetString("UserEmail") ?? string.Empty;

    private string EmployeeId => HttpContext.Session.GetString("EmployeeId") ?? string.Empty;

    private bool IsSignedIn => !string.IsNullOrWhiteSpace(Token);

    public HomeController(HrApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var model = new DashboardViewModel { Role = Role };

        if (RoleIs("Admin", "HR"))
        {
            model.EmployeeCount = await CountFrom("api/Employees", model.Warnings);
        }

        if (RoleIs("Admin", "HR", "Employee"))
        {
            model.DepartmentCount = await CountFrom("api/Departments", model.Warnings);
            model.JobCount = await CountFrom("api/Jobs", model.Warnings);
            model.LocationCount = await CountFrom("api/Locations", model.Warnings);
        }

        if (RoleIs("Manager"))
        {
            model.TeamMembers = await RowsFrom("api/Employees/my-team", model.Warnings);
            model.EmployeeCount = model.TeamMembers.Count;
        }

        return View(model);
    }

    public IActionResult Profile()
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var model = new ProfileViewModel
        {
            Email = Email,
            Role = Role,
            EmployeeId = EmployeeId
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private bool RoleIs(params string[] roles)
    {
        return roles.Any(role => string.Equals(role, Role, StringComparison.OrdinalIgnoreCase));
    }

    private IActionResult? RequireLogin()
    {
        return IsSignedIn ? null : RedirectToAction("Login", "Account");
    }

    private async Task<int> CountFrom(string path, List<string> warnings)
    {
        var rows = await RowsFrom(path, warnings);
        return rows.Count;
    }

    private async Task<IReadOnlyList<JsonElement>> RowsFrom(string path, List<string> warnings)
    {
        var result = await _apiClient.GetAsync(path, Token);
        if (!result.Succeeded)
        {
            warnings.Add(result.ErrorMessage ?? $"{path} is unavailable.");
            return [];
        }

        return ExtractRows(result.Data);
    }

    private static IReadOnlyList<JsonElement> ExtractRows(JsonElement? data)
    {
        if (data is null)
        {
            return [];
        }

        var root = data.Value;

        if (root.ValueKind == JsonValueKind.Array)
        {
            return root.EnumerateArray().Select(item => item.Clone()).ToList();
        }

        if (root.ValueKind == JsonValueKind.Object
            && root.TryGetProperty("data", out var page)
            && page.ValueKind == JsonValueKind.Array)
        {
            return page.EnumerateArray().Select(item => item.Clone()).ToList();
        }

        return [root.Clone()];
    }

}
