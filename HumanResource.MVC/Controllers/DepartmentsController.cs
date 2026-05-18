using HumanResource.MVC.Models.Resources;
using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class DepartmentsController : ModuleControllerBase
{
    public DepartmentsController(HrApiClient apiClient)
        : base(apiClient)
    {
    }

    protected override ApiResourceDefinition GetResource()
    {
        return new ApiResourceDefinition
        {
            Key = "departments",
            Title = "Departments",
            Endpoint = "api/Departments",
            IdField = "departmentId",
            ViewRoles = new[] { "Admin", "HR", "Employee" },
            CreateRoles = new[] { "Admin", "HR" },
            EditRoles = new[] { "Admin", "HR" },
            DeleteRoles = new[] { "Admin" },
            Fields = new List<ApiField>
            {
                new ApiField { Name = "departmentId", Label = "Department", Type = ApiFieldType.Number, ShowInTable = false },
                new ApiField { Name = "departmentName", Label = "Department name", Required = true },
                new ApiField { Name = "managerId", Label = "Manager", Type = ApiFieldType.Number, LookupKey = "employees", ShowInTable = false },
                new ApiField { Name = "locationId", Label = "Location", Type = ApiFieldType.Number, LookupKey = "locations", ShowInTable = false },
                new ApiField { Name = "managerName", Label = "Manager", ReadOnly = true },
                new ApiField { Name = "city", Label = "City", ReadOnly = true }
            },
            Filters = new List<ResourceFilter>
            {
                new ResourceFilter
                {
                    Key = "location",
                    Title = "By location",
                    EndpointTemplate = "api/Departments/location/{locationId}",
                    Roles = new[] { "Admin", "HR", "Employee" },
                    Fields = new List<ApiField> { new ApiField { Name = "locationId", Label = "Location", Type = ApiFieldType.Number, LookupKey = "locations", Required = true } }
                }
            }
        };
    }
}
