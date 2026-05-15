using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class JobHistoryController : ModuleControllerBase
{
    public JobHistoryController(HrApiClient apiClient, ResourceCatalog catalog)
        : base(apiClient, catalog)
    {
    }

    protected override string ResourceKey => "job-history";
}
