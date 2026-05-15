using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class JobsController : ModuleControllerBase
{
    public JobsController(HrApiClient apiClient, ResourceCatalog catalog)
        : base(apiClient, catalog)
    {
    }

    protected override string ResourceKey => "jobs";
}
