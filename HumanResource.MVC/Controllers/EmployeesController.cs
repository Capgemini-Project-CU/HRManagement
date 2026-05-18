using System.Text.Json;
using HumanResource.MVC.Models.Resources;
using HumanResource.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.MVC.Controllers;

public class EmployeesController : Controller
{
    private readonly HrApiClient _apiClient;
    private readonly ModulePageService _modulePages;

    public EmployeesController(HrApiClient apiClient, ModulePageService modulePages)
    {
        _apiClient = apiClient;
        _modulePages = modulePages;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? notice = null, int pageNumber = 1)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = GetResource();
        if (!CanView(resource))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var endpoint = _modulePages.GetListEndpoint(resource, Role, EmployeeId, pageNumber);
        var model = await BuildPageModel(resource, endpoint);
        model.Notice = notice;

        if (_modulePages.UseEmployeePagination(resource, Role))
        {
            model.UsesDefaultPagination = true;
            model.PageNumber = Math.Max(1, pageNumber);
        }

        return View("Index", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create()
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = GetResource();
        if (!CanCreate(resource))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var apiPath = resource.Endpoint;
        var token = Token;

        if (resource.Key == "employees")
        {
            apiPath = "api/Auth/register";
            token = null;
        }

        var payload = _modulePages.BuildCreatePayload(resource, Request.Form);
        var result = await _apiClient.PostAsync(apiPath, payload, token);

        return await RedirectAfterWrite(resource, result, "Record created.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = GetResource();
        if (!CanDelete(resource))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var apiPath = resource.Endpoint + "/" + Uri.EscapeDataString(id);
        var result = await _apiClient.DeleteAsync(apiPath, Token);

        return await RedirectAfterWrite(resource, result, "Record deleted.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Filter(string filterKey)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = GetResource();
        if (!CanView(resource))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var filter = _modulePages.FindFilter(resource, filterKey);
        if (filter is null)
        {
            return NotFound();
        }

        if (!_modulePages.RoleAllowed(Role, filter.Roles))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var endpoint = _modulePages.BuildFilterEndpoint(filter, Request.Form);
        var model = await BuildPageModel(resource, endpoint);
        model.ActiveFilterTitle = filter.Title;

        return View("Index", model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = GetResource();
        if (!CanEdit(resource))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var apiPath = resource.Endpoint + "/" + Uri.EscapeDataString(id);
        var result = await _apiClient.GetAsync(apiPath, Token);
        if (!result.Succeeded || result.Data is null)
        {
            var message = result.ErrorMessage ?? "Record details are unavailable.";
            return RedirectToAction(nameof(Index), new { notice = message });
        }

        var values = _modulePages.ReadValuesFromJson(result.Data.Value, resource);
        var model = await _modulePages.BuildEditModel(resource, id, values, Token);

        return View("Edit", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, IFormCollection form)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = GetResource();
        if (!CanEdit(resource))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var apiPath = resource.Endpoint + "/" + Uri.EscapeDataString(id);
        var payload = _modulePages.BuildEditPayload(resource, id, form);
        var result = await _apiClient.PutAsync(apiPath, payload, Token);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index), new { notice = "Record updated." });
        }

        var values = _modulePages.ReadValuesFromForm(resource, form);
        var model = await _modulePages.BuildEditModel(resource, id, values, Token);
        model.Error = result.ErrorMessage ?? "Record could not be updated.";

        return View("Edit", model);
    }

    private ApiResourceDefinition GetResource()
    {
        return new ApiResourceDefinition
        {
            Key = "employees",
            Title = "Employees",
            Endpoint = "api/Employees",
            IdField = "employeeId",
            ViewRoles = new[] { "Admin", "HR" },
            CreateRoles = new[] { "Admin", "HR" },
            EditRoles = new[] { "Admin", "HR" },
            DeleteRoles = new[] { "Admin" },
            Fields = new List<ApiField>
            {
                new ApiField { Name = "employeeId", Label = "Employee", Type = ApiFieldType.Number, ShowInCreate = false, ShowInEdit = false, IncludeInCreatePayload = false },
                new ApiField { Name = "firstName", Label = "First name", Required = true },
                new ApiField { Name = "lastName", Label = "Last name", Required = true },
                new ApiField { Name = "email", Label = "Email", Required = true },
                new ApiField { Name = "password", Label = "Password", Type = ApiFieldType.Password, Required = true, ShowInTable = false, ShowInEdit = false, IncludeInEditPayload = false },
                new ApiField { Name = "phoneNumber", Label = "Phone" },
                new ApiField { Name = "hireDate", Label = "Hire date", Type = ApiFieldType.Date, Required = true, ShowInCreate = false, IncludeInCreatePayload = false },
                new ApiField { Name = "salary", Label = "Salary", Type = ApiFieldType.Number, Required = true },
                new ApiField { Name = "managerId", Label = "Manager", Type = ApiFieldType.Number, LookupKey = "employees" },
                new ApiField { Name = "departmentId", Label = "Department", Type = ApiFieldType.Number, LookupKey = "departments", Required = true },
                new ApiField { Name = "jobId", Label = "Job", LookupKey = "jobs", Required = true },
                new ApiField { Name = "roleId", Label = "Role", Type = ApiFieldType.Number, LookupKey = "roles", Required = true }
            },
            Filters = new List<ResourceFilter>
            {
                new ResourceFilter
                {
                    Key = "search",
                    Title = "Search",
                    EndpointTemplate = "api/Employees/search?keyword={keyword}",
                    Roles = new[] { "Admin", "HR" },
                    Fields = new List<ApiField> { new ApiField { Name = "keyword", Label = "Keyword", Required = true } }
                },
                new ResourceFilter
                {
                    Key = "department",
                    Title = "By department",
                    EndpointTemplate = "api/Employees/department/{departmentId}",
                    Roles = new[] { "Admin", "HR", "Employee" },
                    Fields = new List<ApiField> { new ApiField { Name = "departmentId", Label = "Department", Type = ApiFieldType.Number, LookupKey = "departments", Required = true } }
                },
                new ResourceFilter
                {
                    Key = "manager",
                    Title = "By manager",
                    EndpointTemplate = "api/Employees/manager/{managerId}",
                    Roles = new[] { "Admin", "HR" },
                    Fields = new List<ApiField> { new ApiField { Name = "managerId", Label = "Manager", Type = ApiFieldType.Number, LookupKey = "employees", Required = true } }
                },
                new ResourceFilter
                {
                    Key = "job",
                    Title = "By job",
                    EndpointTemplate = "api/Employees/job/{jobId}",
                    Roles = new[] { "Admin", "HR", "Employee" },
                    Fields = new List<ApiField> { new ApiField { Name = "jobId", Label = "Job", LookupKey = "jobs", Required = true } }
                },
                new ResourceFilter
                {
                    Key = "role",
                    Title = "By role",
                    EndpointTemplate = "api/Employees/role/{roleId}",
                    Roles = new[] { "Admin" },
                    Fields = new List<ApiField> { new ApiField { Name = "roleId", Label = "Role", Type = ApiFieldType.Number, LookupKey = "roles", Required = true } }
                }
            }
        };
    }
    [HttpGet]
    public async Task<IActionResult> MyTeam()
    {
        var denied = RequireRole("Manager");
        if (denied is not null)
        {
            return denied;
        }

        var team = new ApiResourceDefinition
        {
            Key = "my-team",
            Title = "My Team",
            Endpoint = "api/Employees/my-team",
            IdField = "employeeId",
            ViewRoles = ["Manager"],
            Fields =
            [
                new ApiField { Name = "employeeId", Label = "Employee", Type = ApiFieldType.Number, ShowInTable = false, ReadOnly = true },
                new ApiField { Name = "fullName", Label = "Name", ReadOnly = true },
                new ApiField { Name = "email", Label = "Email", ReadOnly = true },
                new ApiField { Name = "phoneNumber", Label = "Phone", ReadOnly = true },
                new ApiField { Name = "hireDate", Label = "Hire date", Type = ApiFieldType.Date, ReadOnly = true }
            ]
        };

        var model = await _modulePages.BuildPageModel(
            team,
            team.Endpoint,
            Role,
            Token,
            canCreate: false,
            canEdit: false,
            canDelete: false);

        return View("Index", model);
    }

    private string? Token => HttpContext.Session.GetString("JwtToken");

    private string Role => HttpContext.Session.GetString("UserRole") ?? string.Empty;

    private string EmployeeId => HttpContext.Session.GetString("EmployeeId") ?? string.Empty;

    private bool IsSignedIn => !string.IsNullOrWhiteSpace(Token);

    private IActionResult? RequireLogin()
    {
        return IsSignedIn ? null : RedirectToAction("Login", "Account");
    }

    private IActionResult? RequireRole(params string[] roles)
    {
        if (!IsSignedIn)
        {
            return RedirectToAction("Login", "Account");
        }

        return roles.Any(role => string.Equals(role, Role, StringComparison.OrdinalIgnoreCase))
            ? null
            : RedirectToAction("AccessDenied", "Account");
    }

    private bool CanView(ApiResourceDefinition resource)
    {
        return _modulePages.RoleAllowed(Role, resource.ViewRoles);
    }

    private bool CanCreate(ApiResourceDefinition resource)
    {
        return _modulePages.RoleAllowed(Role, resource.CreateRoles);
    }

    private bool CanEdit(ApiResourceDefinition resource)
    {
        return _modulePages.RoleAllowed(Role, resource.EditRoles);
    }

    private bool CanDelete(ApiResourceDefinition resource)
    {
        return _modulePages.RoleAllowed(Role, resource.DeleteRoles);
    }

    private Task<ResourcePageViewModel> BuildPageModel(ApiResourceDefinition resource, string endpoint)
    {
        return _modulePages.BuildPageModel(
            resource,
            endpoint,
            Role,
            Token,
            CanCreate(resource),
            CanEdit(resource),
            CanDelete(resource));
    }

    private async Task<IActionResult> RedirectAfterWrite(
        ApiResourceDefinition resource,
        ApiResult<JsonElement?> result,
        string successMessage)
    {
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index), new { notice = successMessage });
        }

        var model = await BuildPageModel(resource, resource.Endpoint);
        model.Error = result.ErrorMessage ?? "The action could not be completed.";
        return View("Index", model);
    }
}
