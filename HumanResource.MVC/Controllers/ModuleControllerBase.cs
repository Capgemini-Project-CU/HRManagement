using System.Globalization;
using System.Text.Json;
using HumanResource.MVC.Models.Resources;
using HumanResource.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.MVC.Controllers;

public abstract class ModuleControllerBase : MvcControllerBase
{
    private readonly HrApiClient _apiClient;
    private readonly ResourceCatalog _catalog;

    protected ModuleControllerBase(HrApiClient apiClient, ResourceCatalog catalog)
    {
        _apiClient = apiClient;
        _catalog = catalog;
    }

    protected abstract string ResourceKey { get; }

    [HttpGet]
    public virtual async Task<IActionResult> Index(string? notice = null, int pageNumber = 1)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = GetResource();
        if (resource is null)
        {
            return NotFound();
        }

        if (!CanView(resource))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var endpoint = GetListEndpoint(resource, pageNumber);
        var model = await BuildPageModel(resource, endpoint);
        model.Notice = notice;

        if (UseEmployeePagination(resource))
        {
            model.UsesDefaultPagination = true;
            model.PageNumber = Math.Max(1, pageNumber);
        }

        return View("Index", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Create()
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = GetResource();
        if (resource is null)
        {
            return NotFound();
        }

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

        var payload = BuildCreatePayload(resource);
        var result = await _apiClient.PostAsync(apiPath, payload, token);

        return await RedirectAfterWrite(resource, result, "Record created.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Delete(string id)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = GetResource();
        if (resource is null)
        {
            return NotFound();
        }

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
    public virtual async Task<IActionResult> Filter(string filterKey)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = GetResource();
        if (resource is null)
        {
            return NotFound();
        }

        if (!CanView(resource))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var filter = FindFilter(resource, filterKey);
        if (filter is null)
        {
            return NotFound();
        }

        if (!_catalog.IsAllowed(Role, filter.Roles))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var endpoint = BuildFilterEndpoint(filter);
        var model = await BuildPageModel(resource, endpoint);
        model.ActiveFilterTitle = filter.Title;

        return View("Index", model);
    }

    [HttpGet]
    public virtual async Task<IActionResult> Edit(string id)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = GetResource();
        if (resource is null)
        {
            return NotFound();
        }

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

        var values = ReadValuesFromJson(result.Data.Value, resource);
        var model = await BuildEditModel(resource, id, values);

        return View("Edit", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Edit(string id, IFormCollection form)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = GetResource();
        if (resource is null)
        {
            return NotFound();
        }

        if (!CanEdit(resource))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var apiPath = resource.Endpoint + "/" + Uri.EscapeDataString(id);
        var payload = BuildEditPayload(resource, id);
        var result = await _apiClient.PutAsync(apiPath, payload, Token);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index), new { notice = "Record updated." });
        }

        var values = ReadValuesFromForm(resource);
        var model = await BuildEditModel(resource, id, values);
        model.Error = result.ErrorMessage ?? "Record could not be updated.";

        return View("Edit", model);
    }

    protected async Task<IActionResult> TeamIndex(ApiResourceDefinition team)
    {
        var denied = RequireRole("Manager");
        if (denied is not null)
        {
            return denied;
        }

        var model = await BuildPageModel(team, team.Endpoint);
        return View("Index", model);
    }

    private ApiResourceDefinition? GetResource()
    {
        return _catalog.Find(ResourceKey);
    }

    private bool CanView(ApiResourceDefinition resource)
    {
        return _catalog.IsAllowed(Role, resource.ViewRoles);
    }

    private bool CanCreate(ApiResourceDefinition resource)
    {
        return _catalog.IsAllowed(Role, resource.CreateRoles);
    }

    private bool CanEdit(ApiResourceDefinition resource)
    {
        return _catalog.IsAllowed(Role, resource.EditRoles);
    }

    private bool CanDelete(ApiResourceDefinition resource)
    {
        return _catalog.IsAllowed(Role, resource.DeleteRoles);
    }

    private bool RoleIs(params string[] roles)
    {
        foreach (var role in roles)
        {
            if (string.Equals(role, Role, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private bool UseEmployeePagination(ApiResourceDefinition resource)
    {
        return resource.Key == "employees" && RoleIs("Admin", "HR");
    }

    private string GetListEndpoint(ApiResourceDefinition resource, int pageNumber)
    {
        if (UseEmployeePagination(resource))
        {
            var safePageNumber = Math.Max(1, pageNumber);
            return $"api/Employees/pagination?pageNumber={safePageNumber}&pageSize=10";
        }

        if (resource.Key == "job-history"
            && RoleIs("Employee")
            && !string.IsNullOrWhiteSpace(EmployeeId))
        {
            return "api/JobHistory/" + Uri.EscapeDataString(EmployeeId);
        }

        return resource.Endpoint;
    }

    private async Task<ResourcePageViewModel> BuildPageModel(ApiResourceDefinition resource, string endpoint)
    {
        var model = new ResourcePageViewModel();
        model.Resource = resource;
        model.Role = Role;
        model.CanCreate = CanCreate(resource);
        model.CanEdit = CanEdit(resource);
        model.CanDelete = CanDelete(resource);
        model.LookupOptions = await BuildLookupOptions(resource, includeFilterFields: true);

        var result = await _apiClient.GetAsync(endpoint, Token);
        if (!result.Succeeded)
        {
            model.Error = result.ErrorMessage ?? "Records are unavailable.";
            return model;
        }

        model.Records = ExtractRows(result.Data);
        AddPaginationDetails(model, result.Data);

        return model;
    }

    private async Task<ResourceEditViewModel> BuildEditModel(
        ApiResourceDefinition resource,
        string id,
        Dictionary<string, string> values)
    {
        values[resource.IdField] = id;

        var model = new ResourceEditViewModel();
        model.Resource = resource;
        model.Id = id;
        model.Values = values;
        model.LookupOptions = await BuildLookupOptions(resource, includeFilterFields: false);

        return model;
    }

    private Dictionary<string, object?> BuildCreatePayload(ApiResourceDefinition resource)
    {
        var payload = new Dictionary<string, object?>();

        foreach (var field in resource.Fields)
        {
            if (field.ReadOnly || !field.IncludeInCreatePayload)
            {
                continue;
            }

            var value = Request.Form[field.Name].ToString();
            if (!field.ShowInCreate && string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            payload[field.Name] = ConvertValue(field, value);
        }

        return payload;
    }

    private Dictionary<string, object?> BuildEditPayload(ApiResourceDefinition resource, string id)
    {
        var payload = new Dictionary<string, object?>();

        foreach (var field in resource.Fields)
        {
            if (field.ReadOnly || !field.IncludeInEditPayload)
            {
                continue;
            }

            var value = Request.Form[field.Name].ToString();
            if (string.IsNullOrWhiteSpace(value) && field.Name == resource.IdField)
            {
                value = id;
            }

            payload[field.Name] = ConvertValue(field, value);
        }

        return payload;
    }

    private ResourceFilter? FindFilter(ApiResourceDefinition resource, string filterKey)
    {
        foreach (var filter in resource.Filters)
        {
            if (string.Equals(filter.Key, filterKey, StringComparison.OrdinalIgnoreCase))
            {
                return filter;
            }
        }

        return null;
    }

    private string BuildFilterEndpoint(ResourceFilter filter)
    {
        var endpoint = filter.EndpointTemplate;

        foreach (var field in filter.Fields)
        {
            var value = Request.Form[field.Name].ToString();
            endpoint = endpoint.Replace(
                "{" + field.Name + "}",
                Uri.EscapeDataString(value),
                StringComparison.OrdinalIgnoreCase);
        }

        return endpoint;
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

    private async Task<Dictionary<string, List<LookupOption>>> BuildLookupOptions(
        ApiResourceDefinition resource,
        bool includeFilterFields)
    {
        var lookupKeys = GetLookupKeys(resource, includeFilterFields);
        var options = new Dictionary<string, List<LookupOption>>(StringComparer.OrdinalIgnoreCase);

        foreach (var lookupKey in lookupKeys)
        {
            options[lookupKey] = await GetLookupOptions(lookupKey);
        }

        return options;
    }

    private List<string> GetLookupKeys(ApiResourceDefinition resource, bool includeFilterFields)
    {
        var lookupKeys = new List<string>();
        AddLookupKeys(lookupKeys, resource.Fields);

        if (includeFilterFields)
        {
            foreach (var filter in resource.Filters)
            {
                AddLookupKeys(lookupKeys, filter.Fields);
            }
        }

        return lookupKeys;
    }

    private static void AddLookupKeys(List<string> lookupKeys, List<ApiField> fields)
    {
        foreach (var field in fields)
        {
            if (string.IsNullOrWhiteSpace(field.LookupKey))
            {
                continue;
            }

            if (!lookupKeys.Contains(field.LookupKey, StringComparer.OrdinalIgnoreCase))
            {
                lookupKeys.Add(field.LookupKey);
            }
        }
    }

    private async Task<List<LookupOption>> GetLookupOptions(string lookupKey)
    {
        var endpoint = GetLookupEndpoint(lookupKey);
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return [];
        }

        var result = await _apiClient.GetAsync(endpoint, Token);
        if (!result.Succeeded)
        {
            return [];
        }

        var options = new List<LookupOption>();
        foreach (var row in ExtractRows(result.Data))
        {
            var option = CreateLookupOption(lookupKey, row);
            if (!string.IsNullOrWhiteSpace(option.Value))
            {
                options.Add(option);
            }
        }

        return options
            .OrderBy(option => option.Text)
            .ToList();
    }

    private static string GetLookupEndpoint(string lookupKey)
    {
        if (lookupKey == "employees") return "api/Employees";
        if (lookupKey == "departments") return "api/Departments";
        if (lookupKey == "jobs") return "api/Jobs";
        if (lookupKey == "roles") return "api/Roles";
        if (lookupKey == "locations") return "api/Locations";
        if (lookupKey == "regions") return "api/Regions";
        if (lookupKey == "countries") return "api/Countries";

        return string.Empty;
    }

    private static LookupOption CreateLookupOption(string lookupKey, JsonElement row)
    {
        var option = new LookupOption();

        if (lookupKey == "employees")
        {
            option.Value = ReadJsonValue(row, "employeeId");
            option.Text = Join(ReadJsonValue(row, "firstName"), ReadJsonValue(row, "lastName"), ReadJsonValue(row, "email"));
        }
        else if (lookupKey == "departments")
        {
            option.Value = ReadJsonValue(row, "departmentId");
            option.Text = ReadJsonValue(row, "departmentName");
        }
        else if (lookupKey == "jobs")
        {
            option.Value = ReadJsonValue(row, "jobId");
            option.Text = ReadJsonValue(row, "jobTitle");
        }
        else if (lookupKey == "roles")
        {
            option.Value = ReadJsonValue(row, "roleId");
            option.Text = ReadJsonValue(row, "roleName");
        }
        else if (lookupKey == "locations")
        {
            option.Value = ReadJsonValue(row, "locationId");
            option.Text = Join(ReadJsonValue(row, "city"), ReadJsonValue(row, "countryName"));
        }
        else if (lookupKey == "regions")
        {
            option.Value = ReadJsonValue(row, "regionId");
            option.Text = ReadJsonValue(row, "regionName");
        }
        else if (lookupKey == "countries")
        {
            option.Value = ReadJsonValue(row, "countryId");
            option.Text = ReadJsonValue(row, "countryName");
        }

        return option;
    }

    private static Dictionary<string, string> ReadValuesFromJson(JsonElement row, ApiResourceDefinition resource)
    {
        var values = new Dictionary<string, string>();

        foreach (var field in resource.Fields)
        {
            var value = ReadJsonValue(row, field.Name);
            if (field.Type == ApiFieldType.Date && value.Length >= 10)
            {
                value = value.Substring(0, 10);
            }

            values[field.Name] = value;
        }

        return values;
    }

    private Dictionary<string, string> ReadValuesFromForm(ApiResourceDefinition resource)
    {
        var values = new Dictionary<string, string>();

        foreach (var field in resource.Fields)
        {
            if (field.ReadOnly || !field.IncludeInEditPayload)
            {
                continue;
            }

            values[field.Name] = Request.Form[field.Name].ToString();
        }

        return values;
    }

    private static object? ConvertValue(ApiField field, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (field.Type == ApiFieldType.Number)
        {
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number))
            {
                return number;
            }
        }

        return value;
    }

    private static void AddPaginationDetails(ResourcePageViewModel model, JsonElement? data)
    {
        if (data is null || data.Value.ValueKind != JsonValueKind.Object)
        {
            model.TotalRecords = model.Records.Count;
            model.TotalPages = model.Records.Count > 0 ? 1 : 0;
            return;
        }

        var root = data.Value;
        model.TotalRecords = ReadInt(root, "totalRecords", model.Records.Count);
        model.TotalPages = ReadInt(root, "totalPages", model.TotalRecords > 0 ? 1 : 0);
        model.PageNumber = ReadInt(root, "pageNumber", model.PageNumber);
        model.PageSize = ReadInt(root, "pageSize", model.PageSize);
    }

    private static int ReadInt(JsonElement root, string name, int defaultValue)
    {
        if (!root.TryGetProperty(name, out var value))
        {
            return defaultValue;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var number))
        {
            return number;
        }

        if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out number))
        {
            return number;
        }

        return defaultValue;
    }

    private static IReadOnlyList<JsonElement> ExtractRows(JsonElement? data)
    {
        if (data is null)
        {
            return [];
        }

        var root = data.Value;
        if (root.ValueKind == JsonValueKind.Array)
        {
            var rows = new List<JsonElement>();
            foreach (var item in root.EnumerateArray())
            {
                rows.Add(item.Clone());
            }

            return rows;
        }

        if (root.ValueKind == JsonValueKind.Object
            && root.TryGetProperty("data", out var page)
            && page.ValueKind == JsonValueKind.Array)
        {
            var rows = new List<JsonElement>();
            foreach (var item in page.EnumerateArray())
            {
                rows.Add(item.Clone());
            }

            return rows;
        }

        return [root.Clone()];
    }

    private static string ReadJsonValue(JsonElement row, string name)
    {
        if (name == "fullName")
        {
            return Join(ReadJsonValue(row, "firstName"), ReadJsonValue(row, "lastName"));
        }

        if (row.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }

        if (!row.TryGetProperty(name, out var value))
        {
            return string.Empty;
        }

        if (value.ValueKind == JsonValueKind.Null)
        {
            return string.Empty;
        }

        if (value.ValueKind == JsonValueKind.Array)
        {
            var parts = new List<string>();
            foreach (var item in value.EnumerateArray())
            {
                parts.Add(item.ToString());
            }

            return string.Join(", ", parts);
        }

        return value.ToString();
    }

    private static string Join(params string[] parts)
    {
        var visibleParts = new List<string>();

        foreach (var part in parts)
        {
            if (!string.IsNullOrWhiteSpace(part))
            {
                visibleParts.Add(part);
            }
        }

        return string.Join(" - ", visibleParts);
    }
}
