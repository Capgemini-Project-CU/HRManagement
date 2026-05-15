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
            model.RecentJobHistory = await RowsFrom("api/JobHistory", model.Warnings);
        }

        if (RoleIs("Admin"))
        {
            var result = await _apiClient.GetAsync("api/Employees/highest-salary", Token);
            if (result.Succeeded)
            {
                model.HighestSalaryEmployee = result.Data;
            }
            else
            {
                model.Warnings.Add(result.ErrorMessage ?? "Highest salary data is unavailable.");
            }
        }

        if (RoleIs("Manager"))
        {
            model.TeamMembers = await RowsFrom("api/Employees/my-team", model.Warnings);
            model.EmployeeCount = model.TeamMembers.Count;
            model.TeamDepartmentCount = model.TeamMembers
                .Select(member => Value(member, "departmentName"))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();
            model.TeamHiredThisYearCount = model.TeamMembers
                .Count(member => DateTime.TryParse(Value(member, "hireDate"), out var hireDate)
                    && hireDate.Year == DateTime.Today.Year);
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

        var canEdit = RoleIs("Admin", "HR");
        return View(new ProfileViewModel
        {
            Email = Email,
            Role = Role,
            EmployeeId = EmployeeId,
            CanEdit = canEdit,
            Warning = canEdit
                ? null
                : "Profile editing is restricted by the current API permissions."
        });
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

    private static string Value(JsonElement row, string name)
    {
        if (row.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }

        return row.TryGetProperty(name, out var value) ? value.ToString() : string.Empty;
    }
}
