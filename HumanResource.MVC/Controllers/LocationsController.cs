using HumanResource.MVC.Models.Resources;
using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class LocationsController : ModuleControllerBase
{
    public LocationsController(HrApiClient apiClient)
        : base(apiClient)
    {
    }

    protected override ApiResourceDefinition GetResource()
    {
        return new ApiResourceDefinition
        {
            Key = "locations",
            Title = "Locations",
            Endpoint = "api/Locations",
            IdField = "locationId",
            ViewRoles = new[] { "Admin", "HR", "Employee" },
            CreateRoles = new[] { "Admin", "HR" },
            EditRoles = new[] { "Admin", "HR" },
            DeleteRoles = new[] { "Admin" },
            Fields = new List<ApiField>
            {
                new ApiField { Name = "locationId", Label = "Location", Type = ApiFieldType.Number, Required = true, ShowInTable = false },
                new ApiField { Name = "streetAddress", Label = "Street address", Required = true },
                new ApiField { Name = "postalCode", Label = "Postal code", Required = true },
                new ApiField { Name = "city", Label = "City", Required = true },
                new ApiField { Name = "stateProvince", Label = "State", Required = true },
                new ApiField { Name = "countryId", Label = "Country", LookupKey = "countries", Required = true, ShowInTable = false },
                new ApiField { Name = "countryName", Label = "Country", ReadOnly = true }
            },
            Filters = new List<ResourceFilter>
            {
                new ResourceFilter
                {
                    Key = "country",
                    Title = "By country",
                    EndpointTemplate = "api/Locations/country/{countryId}",
                    Roles = new[] { "Admin", "HR", "Employee" },
                    Fields = new List<ApiField> { new ApiField { Name = "countryId", Label = "Country", LookupKey = "countries", Required = true } }
                }
            }
        };
    }
}
