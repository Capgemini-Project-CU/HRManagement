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
        var allowed = FindAllowed(Operation.View);
        if (allowed.Result is not null)
        {
            return allowed.Result;
        }

        var resource = allowed.Value!;
        var endpoint = resource.Endpoint;
        var usesDefaultPagination = string.Equals(resource.Key, "employees", StringComparison.OrdinalIgnoreCase)
            && RoleIs("Admin", "HR");

        if (usesDefaultPagination)
        {
            pageNumber = Math.Max(1, pageNumber);
            endpoint = $"api/Employees/pagination?pageNumber={pageNumber}&pageSize=10";
        }

        if (string.Equals(resource.Key, "job-history", StringComparison.OrdinalIgnoreCase)
            && RoleIs("Employee")
            && !string.IsNullOrWhiteSpace(EmployeeId))
        {
            endpoint = $"api/JobHistory/{Uri.EscapeDataString(EmployeeId)}";
        }

        var model = await BuildPage(resource, endpoint);
        model.UsesDefaultPagination = usesDefaultPagination;
        model.PageNumber = pageNumber;
        model.Notice = notice;
        return View("Index", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Create()
    {
        var allowed = FindAllowed(Operation.Create);
        if (allowed.Result is not null)
        {
            return allowed.Result;
        }

        var resource = allowed.Value!;
        var payload = BuildCreatePayload(resource);
        var createEndpoint = string.Equals(resource.Key, "employees", StringComparison.OrdinalIgnoreCase)
            ? "api/Auth/register"
            : resource.Endpoint;
        var createToken = string.Equals(resource.Key, "employees", StringComparison.OrdinalIgnoreCase)
            ? null
            : Token;
        var result = await _apiClient.PostAsync(createEndpoint, payload, createToken);

        return await RedirectAfterWrite(resource, result, "Record created.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Delete(string id)
    {
        var allowed = FindAllowed(Operation.Delete);
        if (allowed.Result is not null)
        {
            return allowed.Result;
        }

        var resource = allowed.Value!;
        var result = await _apiClient.DeleteAsync($"{resource.Endpoint}/{Uri.EscapeDataString(id)}", Token);
        return await RedirectAfterWrite(resource, result, "Record deleted.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Filter(string filterKey)
    {
        var allowed = FindAllowed(Operation.View);
        if (allowed.Result is not null)
        {
            return allowed.Result;
        }

        var resource = allowed.Value!;
        var filter = resource.Filters.FirstOrDefault(item =>
            string.Equals(item.Key, filterKey, StringComparison.OrdinalIgnoreCase));

        if (filter is null)
        {
            return NotFound();
        }

        if (!_catalog.IsAllowed(Role, filter.Roles))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var model = await BuildPage(resource, BuildEndpoint(filter));
        model.ActiveFilterTitle = filter.Title;
        return View("Index", model);
    }

    [HttpGet]
    public virtual async Task<IActionResult> Edit(string id)
    {
        var allowed = FindAllowed(Operation.Edit);
        if (allowed.Result is not null)
        {
            return allowed.Result;
        }

        var resource = allowed.Value!;
        var result = await _apiClient.GetAsync($"{resource.Endpoint}/{Uri.EscapeDataString(id)}", Token);
        if (!result.Succeeded || result.Data is null)
        {
            return RedirectToAction(nameof(Index), new
            {
                notice = result.ErrorMessage ?? "Record details are unavailable."
            });
        }

        return View("Edit", await BuildEditModel(resource, id, ReadValues(result.Data.Value, resource)));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Edit(string id, IFormCollection _)
    {
        var allowed = FindAllowed(Operation.Edit);
        if (allowed.Result is not null)
        {
            return allowed.Result;
        }

        var resource = allowed.Value!;
        var result = await _apiClient.PutAsync(
            $"{resource.Endpoint}/{Uri.EscapeDataString(id)}",
            BuildEditPayload(resource, id),
            Token);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index), new { notice = "Record updated." });
        }

        var model = await BuildEditModel(resource, id, ReadPostedValues(resource));
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

        return View("Index", await BuildPage(team, team.Endpoint));
    }

    private (ApiResourceDefinition? Value, IActionResult? Result) FindAllowed(Operation operation)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return (null, login);
        }

        var resource = _catalog.Find(ResourceKey);
        if (resource is null)
        {
            return (null, NotFound());
        }

        var roles = operation switch
        {
            Operation.Create => resource.CreateRoles,
            Operation.Edit => resource.EditRoles,
            Operation.Delete => resource.DeleteRoles,
            _ => resource.ViewRoles
        };

        return _catalog.IsAllowed(Role, roles)
            ? (resource, null)
            : (null, RedirectToAction("AccessDenied", "Account"));
    }

    private bool RoleIs(params string[] roles)
    {
        return roles.Any(role => string.Equals(role, Role, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<ResourcePageViewModel> BuildPage(ApiResourceDefinition resource, string endpoint)
    {
        var result = await _apiClient.GetAsync(endpoint, Token);
        var model = new ResourcePageViewModel
        {
            Resource = resource,
            Role = Role,
            LookupOptions = await BuildLookupOptions(resource, includeFilters: true),
            CanCreate = _catalog.IsAllowed(Role, resource.CreateRoles),
            CanEdit = _catalog.IsAllowed(Role, resource.EditRoles),
            CanDelete = _catalog.IsAllowed(Role, resource.DeleteRoles)
        };

        if (!result.Succeeded)
        {
            model.Error = result.ErrorMessage ?? "Records are unavailable.";
            return model;
        }

        model.Records = ExtractRows(result.Data);
        ApplyPaginationMetadata(model, result.Data);
        return model;
    }

    private static void ApplyPaginationMetadata(ResourcePageViewModel model, JsonElement? data)
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

    private async Task<ResourceEditViewModel> BuildEditModel(
        ApiResourceDefinition resource,
        string id,
        Dictionary<string, string> values)
    {
        values[resource.IdField] = id;

        return new ResourceEditViewModel
        {
            Resource = resource,
            Id = id,
            Values = values,
            LookupOptions = await BuildLookupOptions(resource, includeFilters: false)
        };
    }

    private Dictionary<string, object?> BuildCreatePayload(ApiResourceDefinition resource)
    {
        var payload = new Dictionary<string, object?>();

        foreach (var field in resource.Fields.Where(field => !field.ReadOnly && field.IncludeInCreatePayload))
        {
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

        foreach (var field in resource.Fields.Where(field => !field.ReadOnly && field.IncludeInEditPayload))
        {
            var value = Request.Form[field.Name].ToString();
            if (string.IsNullOrWhiteSpace(value)
                && string.Equals(field.Name, resource.IdField, StringComparison.OrdinalIgnoreCase))
            {
                value = id;
            }

            payload[field.Name] = ConvertValue(field, value);
        }

        return payload;
    }

    private Dictionary<string, string> ReadPostedValues(ApiResourceDefinition resource)
    {
        return resource.Fields
            .Where(field => !field.ReadOnly && field.IncludeInEditPayload)
            .ToDictionary(field => field.Name, field => Request.Form[field.Name].ToString());
    }

    private string BuildEndpoint(ResourceFilter filter)
    {
        var endpoint = filter.EndpointTemplate;

        foreach (var field in filter.Fields)
        {
            endpoint = endpoint.Replace(
                "{" + field.Name + "}",
                Uri.EscapeDataString(Request.Form[field.Name].ToString()),
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

        var model = await BuildPage(resource, resource.Endpoint);
        model.Error = result.ErrorMessage ?? "The action could not be completed.";
        return View("Index", model);
    }

    private async Task<Dictionary<string, List<LookupOption>>> BuildLookupOptions(
        ApiResourceDefinition resource,
        bool includeFilters)
    {
        var fields = includeFilters
            ? resource.Fields.Concat(resource.Filters.SelectMany(filter => filter.Fields))
            : resource.Fields;
        var lookupKeys = fields
            .Select(field => field.LookupKey)
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Distinct(StringComparer.OrdinalIgnoreCase);

        var options = new Dictionary<string, List<LookupOption>>(StringComparer.OrdinalIgnoreCase);
        foreach (var lookupKey in lookupKeys)
        {
            options[lookupKey!] = await GetLookupOptions(lookupKey!);
        }

        return options;
    }

    private async Task<List<LookupOption>> GetLookupOptions(string lookupKey)
    {
        var endpoint = lookupKey switch
        {
            "employees" => "api/Employees",
            "departments" => "api/Departments",
            "jobs" => "api/Jobs",
            "roles" => "api/Roles",
            "locations" => "api/Locations",
            "regions" => "api/Regions",
            "countries" => "api/Countries",
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return [];
        }

        var result = await _apiClient.GetAsync(endpoint, Token);
        return result.Succeeded
            ? ExtractRows(result.Data)
                .Select(row => CreateLookupOption(lookupKey, row))
                .Where(option => !string.IsNullOrWhiteSpace(option.Value))
                .OrderBy(option => option.Text)
                .ToList()
            : [];
    }

    private static Dictionary<string, string> ReadValues(JsonElement row, ApiResourceDefinition resource)
    {
        return resource.Fields.ToDictionary(field => field.Name, field => InputValue(row, field));
    }

    private static object? ConvertValue(ApiField field, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return field.Type == ApiFieldType.Number
            && decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number)
                ? number
                : value;
    }

    private static LookupOption CreateLookupOption(string lookupKey, JsonElement row)
    {
        return lookupKey switch
        {
            "employees" => new LookupOption { Value = Value(row, "employeeId"), Text = Join(Value(row, "firstName"), Value(row, "lastName"), Value(row, "email")) },
            "departments" => new LookupOption { Value = Value(row, "departmentId"), Text = Value(row, "departmentName") },
            "jobs" => new LookupOption { Value = Value(row, "jobId"), Text = Value(row, "jobTitle") },
            "roles" => new LookupOption { Value = Value(row, "roleId"), Text = Value(row, "roleName") },
            "locations" => new LookupOption { Value = Value(row, "locationId"), Text = Join(Value(row, "city"), Value(row, "countryName")) },
            "regions" => new LookupOption { Value = Value(row, "regionId"), Text = Value(row, "regionName") },
            "countries" => new LookupOption { Value = Value(row, "countryId"), Text = Value(row, "countryName") },
            _ => new LookupOption()
        };
    }

    private static string InputValue(JsonElement row, ApiField field)
    {
        var value = Value(row, field.Name);
        return field.Type == ApiFieldType.Date && value.Length >= 10 ? value[..10] : value;
    }

    private static int ReadInt(JsonElement root, string name, int fallback)
    {
        if (!root.TryGetProperty(name, out var value))
        {
            return fallback;
        }

        return value.ValueKind switch
        {
            JsonValueKind.Number when value.TryGetInt32(out var number) => number,
            JsonValueKind.String when int.TryParse(value.GetString(), out var number) => number,
            _ => fallback
        };
    }

    private static string Value(JsonElement row, string name)
    {
        if (string.Equals(name, "fullName", StringComparison.OrdinalIgnoreCase))
        {
            return Join(Value(row, "firstName"), Value(row, "lastName"));
        }

        if (row.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }

        return row.TryGetProperty(name, out var value) ? Display(value) : string.Empty;
    }

    private static string Display(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Array => string.Join(", ", value.EnumerateArray().Select(Display)),
            JsonValueKind.Null => string.Empty,
            _ => value.ToString()
        };
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
            return root.EnumerateArray().Select(item => item.Clone()).ToList();
        }

        return root.ValueKind == JsonValueKind.Object
            && root.TryGetProperty("data", out var page)
            && page.ValueKind == JsonValueKind.Array
                ? page.EnumerateArray().Select(item => item.Clone()).ToList()
                : [root.Clone()];
    }

    private static string Join(params string[] parts)
    {
        return string.Join(" - ", parts.Where(part => !string.IsNullOrWhiteSpace(part)));
    }

    private enum Operation
    {
        View,
        Create,
        Edit,
        Delete
    }
}
