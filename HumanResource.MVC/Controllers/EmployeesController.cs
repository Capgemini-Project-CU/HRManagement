using HumanResource.MVC.Models.Resources;
using HumanResource.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.MVC.Controllers;

public class EmployeesController : ModuleControllerBase
{
    public EmployeesController(HrApiClient apiClient)
        : base(apiClient)
    {
    }

    protected override ApiResourceDefinition GetResource()
    {
        return new ApiResourceDefinition
        {
            Key = "employees",
            Title = "Employees",
            Endpoint = "api/Employees",
            IdField = "employeeId",
            ViewRoles = new[] { "Admin", "HR" },
            CreateRoles = new[] { "Admin", "HR" },
            EditRoles = new[] { "Admin", "HR" },
            DeleteRoles = new[] { "Admin" },
            Fields = new List<ApiField>
            {
                new ApiField { Name = "employeeId", Label = "Employee", Type = ApiFieldType.Number, ShowInCreate = false, ShowInEdit = false, IncludeInCreatePayload = false },
                new ApiField { Name = "firstName", Label = "First name", Required = true },
                new ApiField { Name = "lastName", Label = "Last name", Required = true },
                new ApiField { Name = "email", Label = "Email", Required = true },
                new ApiField { Name = "password", Label = "Password", Type = ApiFieldType.Password, Required = true, ShowInTable = false, ShowInEdit = false, IncludeInEditPayload = false },
                new ApiField { Name = "phoneNumber", Label = "Phone" },
                new ApiField { Name = "hireDate", Label = "Hire date", Type = ApiFieldType.Date, Required = true, ShowInCreate = false, IncludeInCreatePayload = false },
                new ApiField { Name = "salary", Label = "Salary", Type = ApiFieldType.Number, Required = true },
                new ApiField { Name = "managerId", Label = "Manager", Type = ApiFieldType.Number, LookupKey = "employees" },
                new ApiField { Name = "departmentId", Label = "Department", Type = ApiFieldType.Number, LookupKey = "departments", Required = true },
                new ApiField { Name = "jobId", Label = "Job", LookupKey = "jobs", Required = true },
                new ApiField { Name = "roleId", Label = "Role", Type = ApiFieldType.Number, LookupKey = "roles", Required = true }
            },
            Filters = new List<ResourceFilter>
            {
                new ResourceFilter
                {
                    Key = "search",
                    Title = "Search",
                    EndpointTemplate = "api/Employees/search?keyword={keyword}",
                    Roles = new[] { "Admin", "HR" },
                    Fields = new List<ApiField> { new ApiField { Name = "keyword", Label = "Keyword", Required = true } }
                },
                new ResourceFilter
                {
                    Key = "department",
                    Title = "By department",
                    EndpointTemplate = "api/Employees/department/{departmentId}",
                    Roles = new[] { "Admin", "HR", "Employee" },
                    Fields = new List<ApiField> { new ApiField { Name = "departmentId", Label = "Department", Type = ApiFieldType.Number, LookupKey = "departments", Required = true } }
                },
                new ResourceFilter
                {
                    Key = "manager",
                    Title = "By manager",
                    EndpointTemplate = "api/Employees/manager/{managerId}",
                    Roles = new[] { "Admin", "HR" },
                    Fields = new List<ApiField> { new ApiField { Name = "managerId", Label = "Manager", Type = ApiFieldType.Number, LookupKey = "employees", Required = true } }
                },
                new ResourceFilter
                {
                    Key = "job",
                    Title = "By job",
                    EndpointTemplate = "api/Employees/job/{jobId}",
                    Roles = new[] { "Admin", "HR", "Employee" },
                    Fields = new List<ApiField> { new ApiField { Name = "jobId", Label = "Job", LookupKey = "jobs", Required = true } }
                },
                new ResourceFilter
                {
                    Key = "role",
                    Title = "By role",
                    EndpointTemplate = "api/Employees/role/{roleId}",
                    Roles = new[] { "Admin" },
                    Fields = new List<ApiField> { new ApiField { Name = "roleId", Label = "Role", Type = ApiFieldType.Number, LookupKey = "roles", Required = true } }
                }
            }
        };
    }

    [HttpGet]
    public async Task<IActionResult> MyTeam()
    {
        var team = new ApiResourceDefinition
        {
            Key = "my-team",
            Title = "My Team",
            Endpoint = "api/Employees/my-team",
            IdField = "employeeId",
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
