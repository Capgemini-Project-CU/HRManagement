using HumanResource.MVC.Models.Resources;
using HumanResource.MVC.Services;

namespace HumanResource.MVC.Controllers;

public class RolesController : ModuleControllerBase
{
    public RolesController(HrApiClient apiClient)
        : base(apiClient)
    {
    }

    protected override ApiResourceDefinition GetResource()
    {
        return new ApiResourceDefinition
        {
            Key = "roles",
            Title = "Roles",
            Endpoint = "api/Roles",
            IdField = "roleId",
            ViewRoles = new[] { "Admin", "HR" },
            CreateRoles = new[] { "Admin" },
            EditRoles = new[] { "Admin" },
            DeleteRoles = new[] { "Admin" },
            Fields = new List<ApiField>
            {
                new ApiField { Name = "roleId", Label = "Role", Type = ApiFieldType.Number, Required = true, ShowInTable = false },
                new ApiField { Name = "roleName", Label = "Role name", Required = true }
            }
        };
    }
}
