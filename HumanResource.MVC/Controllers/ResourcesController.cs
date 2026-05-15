using System.Globalization;
using System.Text.Json;
using HumanResource.MVC.Models.Resources;
using HumanResource.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.MVC.Controllers;

[Route("[controller]")]
public class ResourcesController : MvcControllerBase
{
    private readonly HrApiClient _apiClient;
    private readonly ResourceCatalog _catalog;

    public ResourcesController(HrApiClient apiClient, ResourceCatalog catalog)
    {
        _apiClient = apiClient;
        _catalog = catalog;
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> Index(string key, string? notice = null)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = _catalog.Find(key);
        if (resource is null)
        {
            return NotFound();
        }

        if (!_catalog.IsAllowed(Role, resource.ViewRoles))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var model = await BuildPage(resource, resource.Endpoint);
        model.Notice = notice;
        return View(model);
    }

    [HttpGet("MyTeam")]
    public async Task<IActionResult> MyTeam()
    {
        var denied = RequireRole("Manager");
        if (denied is not null)
        {
            return denied;
        }

        var employees = _catalog.Find("employees");
        if (employees is null)
        {
            return NotFound();
        }

        var team = new ApiResourceDefinition
        {
            Key = "my-team",
            Title = "My Team",
            Endpoint = "api/Employees/my-team",
            IdField = "employeeId",
            Summary = "Direct reports assigned to you.",
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

        return View("Index", await BuildPage(team, team.Endpoint));
    }

    [HttpPost("{key}/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string key)
    {
        var resource = FindAllowed(key, Operation.Create);
        if (resource.Result is not null)
        {
            return resource.Result;
        }

        var payload = BuildPayload(resource.Value!, isUpdate: false);
        var createEndpoint = string.Equals(resource.Value!.Key, "employees", StringComparison.OrdinalIgnoreCase)
            ? "api/Auth/register"
            : resource.Value.Endpoint;
        var createToken = string.Equals(resource.Value.Key, "employees", StringComparison.OrdinalIgnoreCase)
            ? null
            : Token;
        var result = await _apiClient.PostAsync(createEndpoint, payload, createToken);

        return await RedirectAfterWrite(resource.Value, result, "Record created.");
    }

    [HttpPost("{key}/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string key, string id)
    {
        var resource = FindAllowed(key, Operation.Edit);
        if (resource.Result is not null)
        {
            return resource.Result;
        }

        var payload = BuildPayload(resource.Value!, isUpdate: true, id);
        var path = $"{resource.Value!.Endpoint}/{Uri.EscapeDataString(id)}";
        var result = await _apiClient.PutAsync(path, payload, Token);

        return await RedirectAfterWrite(resource.Value, result, "Record updated.");
    }

    [HttpPost("{key}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string key, string id)
    {
        var resource = FindAllowed(key, Operation.Delete);
        if (resource.Result is not null)
        {
            return resource.Result;
        }

        var path = $"{resource.Value!.Endpoint}/{Uri.EscapeDataString(id)}";
        var result = await _apiClient.DeleteAsync(path, Token);

        return await RedirectAfterWrite(resource.Value, result, "Record deleted.");
    }

    [HttpPost("{key}/filter/{filterKey}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Filter(string key, string filterKey)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return login;
        }

        var resource = _catalog.Find(key);
        if (resource is null)
        {
            return NotFound();
        }

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

        var endpoint = BuildEndpoint(filter);
        var model = await BuildPage(resource, endpoint);
        model.ActiveFilterTitle = filter.Title;
        return View("Index", model);
    }

    private (ApiResourceDefinition? Value, IActionResult? Result) FindAllowed(string key, Operation operation)
    {
        var login = RequireLogin();
        if (login is not null)
        {
            return (null, login);
        }

        var resource = _catalog.Find(key);
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

    private async Task<ResourcePageViewModel> BuildPage(ApiResourceDefinition resource, string endpoint)
    {
        var result = await _apiClient.GetAsync(endpoint, Token);
        var model = new ResourcePageViewModel
        {
            Resource = resource,
            Role = Role,
            LookupOptions = await BuildLookupOptions(resource),
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
        return model;
    }

    private async Task<IActionResult> RedirectAfterWrite(
        ApiResourceDefinition resource,
        ApiResult<JsonElement?> result,
        string successMessage)
    {
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index), new { key = resource.Key, notice = successMessage });
        }

        var model = await BuildPage(resource, resource.Endpoint);
        model.Error = result.ErrorMessage ?? "The action could not be completed.";
        return View("Index", model);
    }

    private Dictionary<string, object?> BuildPayload(
        ApiResourceDefinition resource,
        bool isUpdate,
        string? id = null)
    {
        var payload = new Dictionary<string, object?>();

        foreach (var field in resource.Fields.Where(field => !field.ReadOnly))
        {
            var include = isUpdate ? field.IncludeInEditPayload : field.IncludeInCreatePayload;
            var visible = isUpdate ? field.ShowInEdit : field.ShowInCreate;

            if (!include)
            {
                continue;
            }

            var value = Request.Form[field.Name].ToString();
            if (string.IsNullOrWhiteSpace(value)
                && isUpdate
                && string.Equals(field.Name, resource.IdField, StringComparison.OrdinalIgnoreCase))
            {
                value = id ?? string.Empty;
            }

            if (!visible && string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            payload[field.Name] = ConvertValue(field, value);
        }

        return payload;
    }

    private static object? ConvertValue(ApiField field, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return field.Type switch
        {
            ApiFieldType.Number => decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number)
                ? number
                : value,
            _ => value
        };
    }

    private string BuildEndpoint(ResourceFilter filter)
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

    private async Task<Dictionary<string, List<LookupOption>>> BuildLookupOptions(ApiResourceDefinition resource)
    {
        var lookupKeys = resource.Fields
            .Concat(resource.Filters.SelectMany(filter => filter.Fields))
            .Select(field => field.LookupKey)
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

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
        if (!result.Succeeded)
        {
            return [];
        }

        return ExtractRows(result.Data)
            .Select(row => CreateLookupOption(lookupKey, row))
            .Where(option => !string.IsNullOrWhiteSpace(option.Value))
            .OrderBy(option => option.Text)
            .ToList();
    }

    private static LookupOption CreateLookupOption(string lookupKey, JsonElement row)
    {
        return lookupKey switch
        {
            "employees" => new LookupOption
            {
                Value = Value(row, "employeeId"),
                Text = Join(Value(row, "firstName"), Value(row, "lastName"), Value(row, "email"))
            },
            "departments" => new LookupOption
            {
                Value = Value(row, "departmentId"),
                Text = Value(row, "departmentName")
            },
            "jobs" => new LookupOption
            {
                Value = Value(row, "jobId"),
                Text = Value(row, "jobTitle")
            },
            "roles" => new LookupOption
            {
                Value = Value(row, "roleId"),
                Text = Value(row, "roleName")
            },
            "locations" => new LookupOption
            {
                Value = Value(row, "locationId"),
                Text = Join(Value(row, "city"), Value(row, "countryName"))
            },
            "regions" => new LookupOption
            {
                Value = Value(row, "regionId"),
                Text = Value(row, "regionName")
            },
            "countries" => new LookupOption
            {
                Value = Value(row, "countryId"),
                Text = Value(row, "countryName")
            },
            _ => new LookupOption()
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

    private static string Join(params string[] parts)
    {
        return string.Join(" - ", parts.Where(part => !string.IsNullOrWhiteSpace(part)));
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

        if (root.ValueKind == JsonValueKind.Object
            && root.TryGetProperty("data", out var page)
            && page.ValueKind == JsonValueKind.Array)
        {
            return page.EnumerateArray().Select(item => item.Clone()).ToList();
        }

        return [root.Clone()];
    }

    private enum Operation
    {
        Create,
        Edit,
        Delete
    }
}
