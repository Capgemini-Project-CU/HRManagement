using System.Text.Json;

namespace HumanResource.MVC.Models.Dashboard;

public class DashboardViewModel
{
    public string Role { get; set; } = string.Empty;

    public int EmployeeCount { get; set; }

    public int DepartmentCount { get; set; }

    public int JobCount { get; set; }

    public int LocationCount { get; set; }

    public int TeamDepartmentCount { get; set; }

    public int TeamHiredThisYearCount { get; set; }

    public JsonElement? HighestSalaryEmployee { get; set; }

    public IReadOnlyList<JsonElement> TeamMembers { get; set; } = [];

    public IReadOnlyList<JsonElement> RecentJobHistory { get; set; } = [];

    public List<string> Warnings { get; set; } = [];
}
