using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class CountriesController : ModuleControllerBase
{
    public CountriesController(HrApiClient apiClient, ResourceCatalog catalog)
        : base(apiClient, catalog)
    {
    }

    protected override string ResourceKey => "countries";
}
