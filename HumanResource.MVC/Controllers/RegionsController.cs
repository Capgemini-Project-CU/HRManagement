using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class RegionsController : ModuleControllerBase
{
    public RegionsController(HrApiClient apiClient, ResourceCatalog catalog)
        : base(apiClient, catalog)
    {
    }

    protected override string ResourceKey => "regions";
}
