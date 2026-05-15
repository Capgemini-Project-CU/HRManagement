using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class RolesController : ModuleControllerBase
{
    public RolesController(HrApiClient apiClient, ResourceCatalog catalog)
        : base(apiClient, catalog)
    {
    }

    protected override string ResourceKey => "roles";
}
