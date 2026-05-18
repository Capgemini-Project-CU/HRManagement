using HumanResource.MVC.Models.Resources;
using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class CountriesController : ModuleControllerBase
{
    public CountriesController(HrApiClient apiClient)
        : base(apiClient)
    {
    }

    protected override ApiResourceDefinition GetResource()
    {
        return new ApiResourceDefinition
        {
            Key = "countries",
            Title = "Countries",
            Endpoint = "api/Countries",
            IdField = "countryId",
            ViewRoles = new[] { "Admin", "HR" },
            CreateRoles = new[] { "Admin", "HR" },
            EditRoles = new[] { "Admin", "HR" },
            DeleteRoles = new[] { "Admin" },
            Fields = new List<ApiField>
            {
                new ApiField { Name = "countryId", Label = "Country code", Required = true, ShowInTable = false },
                new ApiField { Name = "countryName", Label = "Country name", Required = true },
                new ApiField { Name = "regionId", Label = "Region", Type = ApiFieldType.Number, LookupKey = "regions", Required = true },
                new ApiField { Name = "regionName", Label = "Region", ReadOnly = true }
            },
            Filters = new List<ResourceFilter>
            {
                new ResourceFilter
                {
                    Key = "region",
                    Title = "By region",
                    EndpointTemplate = "api/Countries/region/{regionId}",
                    Roles = new[] { "Admin", "HR" },
                    Fields = new List<ApiField> { new ApiField { Name = "regionId", Label = "Region", Type = ApiFieldType.Number, LookupKey = "regions", Required = true } }
                }
            }
        };
    }
}
