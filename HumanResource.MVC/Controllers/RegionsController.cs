using HumanResource.MVC.Models.Resources;
using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class RegionsController : ModuleControllerBase
{
    public RegionsController(HrApiClient apiClient)
        : base(apiClient)
    {
    }

    protected override ApiResourceDefinition GetResource()
    {
        return new ApiResourceDefinition
        {
            Key = "regions",
            Title = "Regions",
            Endpoint = "api/Regions",
            IdField = "regionId",
            ViewRoles = new[] { "Admin", "HR", "Employee" },
            CreateRoles = new[] { "Admin", "HR" },
            EditRoles = new[] { "Admin", "HR" },
            DeleteRoles = new[] { "Admin" },
            Fields = new List<ApiField>
            {
                new ApiField { Name = "regionId", Label = "Region", Type = ApiFieldType.Number, Required = true, ShowInTable = false },
                new ApiField { Name = "regionName", Label = "Region name", Required = true },
                new ApiField { Name = "countryNames", Label = "Countries", ReadOnly = true }
            }
        };
    }
}
