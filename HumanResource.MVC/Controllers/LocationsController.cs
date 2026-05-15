using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class LocationsController : ModuleControllerBase
{
    public LocationsController(HrApiClient apiClient, ResourceCatalog catalog)
        : base(apiClient, catalog)
    {
    }

    protected override string ResourceKey => "locations";
}
