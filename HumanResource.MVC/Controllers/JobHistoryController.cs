using HumanResource.MVC.Models.Resources;
using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class JobHistoryController : ModuleControllerBase
{
    public JobHistoryController(HrApiClient apiClient)
        : base(apiClient)
    {
    }

    protected override ApiResourceDefinition GetResource()
    {
        return new ApiResourceDefinition
        {
            Key = "job-history",
            Title = "Job History",
            Endpoint = "api/JobHistory",
            IdField = "employeeId",
            ViewRoles = new[] { "Admin", "HR", "Employee" },
            CreateRoles = new[] { "Admin", "HR" },
            EditRoles = Array.Empty<string>(),
            DeleteRoles = new[] { "Admin" },
            Fields = new List<ApiField>
            {
                new ApiField { Name = "employeeId", Label = "Employee", Type = ApiFieldType.Number, LookupKey = "employees", Required = true },
                new ApiField { Name = "startDate", Label = "Start date", Type = ApiFieldType.Date, Required = true },
                new ApiField { Name = "endDate", Label = "End date", Type = ApiFieldType.Date, Required = true },
                new ApiField { Name = "jobId", Label = "Job", LookupKey = "jobs", Required = true },
                new ApiField { Name = "departmentId", Label = "Department", Type = ApiFieldType.Number, LookupKey = "departments", Required = true }
            },
            Filters = new List<ResourceFilter>
            {
                new ResourceFilter
                {
                    Key = "employee",
                    Title = "By employee",
                    EndpointTemplate = "api/JobHistory/{employeeId}",
                    Roles = new[] { "Admin", "HR", "Employee" },
                    Fields = new List<ApiField> { new ApiField { Name = "employeeId", Label = "Employee", Type = ApiFieldType.Number, LookupKey = "employees", Required = true } }
                },
                new ResourceFilter
                {
                    Key = "department",
                    Title = "By department",
                    EndpointTemplate = "api/JobHistory/department/{departmentId}",
                    Roles = new[] { "Admin", "HR", "Employee" },
                    Fields = new List<ApiField> { new ApiField { Name = "departmentId", Label = "Department", Type = ApiFieldType.Number, LookupKey = "departments", Required = true } }
                }
            }
        };
    }
}
