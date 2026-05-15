using HumanResource.MVC.Models.Resources;
using HumanResource.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.MVC.Controllers;

public class EmployeesController : ModuleControllerBase
{
    public EmployeesController(HrApiClient apiClient, ResourceCatalog catalog)
        : base(apiClient, catalog)
    {
    }

    protected override string ResourceKey => "employees";

    [HttpGet]
    public async Task<IActionResult> MyTeam()
    {
        var team = new ApiResourceDefinition
        {
            Key = "my-team",
            Title = "My Team",
            Endpoint = "api/Employees/my-team",
            IdField = "employeeId",
            Summary = "Direct reports assigned to you.",
            ViewRoles = ["Manager"],
            Fields =
            [
                new ApiField { Name = "employeeId", Label = "Employee", Type = ApiFieldType.Number, ShowInTable = false, ReadOnly = true },
                new ApiField { Name = "fullName", Label = "Name", ReadOnly = true },
                new ApiField { Name = "email", Label = "Email", ReadOnly = true },
                new ApiField { Name = "phoneNumber", Label = "Phone", ReadOnly = true },
                new ApiField { Name = "hireDate", Label = "Hire date", Type = ApiFieldType.Date, ReadOnly = true }
            ]
        };

        return await TeamIndex(team);
    }
}
