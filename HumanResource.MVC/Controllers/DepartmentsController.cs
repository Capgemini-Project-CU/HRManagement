using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class DepartmentsController : ModuleControllerBase
{
    public DepartmentsController(HrApiClient apiClient, ResourceCatalog catalog)
        : base(apiClient, catalog)
    {
    }

    protected override string ResourceKey => "departments";
}
