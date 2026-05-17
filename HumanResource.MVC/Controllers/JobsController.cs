using HumanResource.MVC.Models.Resources;
using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class JobsController : ModuleControllerBase
{
    public JobsController(HrApiClient apiClient)
        : base(apiClient)
    {
    }

    protected override ApiResourceDefinition GetResource()
    {
        return new ApiResourceDefinition
        {
            Key = "jobs",
            Title = "Jobs",
            Endpoint = "api/Jobs",
            IdField = "jobId",
            ViewRoles = new[] { "Admin", "HR", "Employee" },
            CreateRoles = new[] { "Admin", "HR" },
            EditRoles = new[] { "Admin", "HR" },
            DeleteRoles = new[] { "Admin" },
            Fields = new List<ApiField>
            {
                new ApiField { Name = "jobId", Label = "Job code", Required = true, ShowInTable = false },
                new ApiField { Name = "jobTitle", Label = "Job title", Required = true },
                new ApiField { Name = "minSalary", Label = "Min salary", Type = ApiFieldType.Number },
                new ApiField { Name = "maxSalary", Label = "Max salary", Type = ApiFieldType.Number }
            },
            Filters = new List<ResourceFilter>
            {
                new ResourceFilter
                {
                    Key = "salary-range",
                    Title = "Salary range",
                    EndpointTemplate = "api/Jobs/salary-range?min={min}&max={max}",
                    Roles = new[] { "Admin", "HR", "Employee" },
                    Fields = new List<ApiField>
                    {
                        new ApiField { Name = "min", Label = "Minimum", Type = ApiFieldType.Number, Required = true },
                        new ApiField { Name = "max", Label = "Maximum", Type = ApiFieldType.Number, Required = true }
                    }
                }
            }
        };
    }
}
