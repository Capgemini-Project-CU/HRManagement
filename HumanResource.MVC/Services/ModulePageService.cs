using System.Globalization;
using System.Text.Json;
using HumanResource.MVC.Models.Resources;

namespace HumanResource.MVC.Services;

public class ModulePageService
{
    private readonly HrApiClient _apiClient;

    public ModulePageService(HrApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public bool RoleAllowed(string role, IEnumerable<string> allowedRoles)
    {
        foreach (var allowedRole in allowedRoles)
        {
            if (string.Equals(allowedRole, role, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public bool UseEmployeePagination(ApiResourceDefinition resource, string role)
    {
        return resource.Key == "employees" && RoleIs(role, "Admin", "HR");
    }

    public string GetListEndpoint(
        ApiResourceDefinition resource,
        string role,
        string employeeId,
        int pageNumber)
    {
        if (UseEmployeePagination(resource, role))
        {
            var safePageNumber = Math.Max(1, pageNumber);
            return $"api/Employees/pagination?pageNumber={safePageNumber}&pageSize=10";
        }

        if (resource.Key == "job-history"
            && RoleIs(role, "Employee")
            && !string.IsNullOrWhiteSpace(employeeId))
        {
            return "api/JobHistory/" + Uri.EscapeDataString(employeeId);
        }

        return resource.Endpoint;
    }

    public async Task<ResourcePageViewModel> BuildPageModel(
        ApiResourceDefinition resource,
        string endpoint,
        string role,
        string? token,
        bool canCreate,
        bool canEdit,
        bool canDelete)
    {
        var model = new ResourcePageViewModel
        {
            Resource = resource,
            Role = role,
            CanCreate = canCreate,
            CanEdit = canEdit,
            CanDelete = canDelete,
            LookupOptions = await BuildLookupOptions(resource, token, includeFilterFields: true)
        };

        var result = await _apiClient.GetAsync(endpoint, token);
        if (!result.Succeeded)
        {
            model.Error = result.ErrorMessage ?? "Records are unavailable.";
            return model;
        }

        model.Records = ExtractRows(result.Data);
        AddPaginationDetails(model, result.Data);

        return model;
    }

    public async Task<ResourceEditViewModel> BuildEditModel(
        ApiResourceDefinition resource,
        string id,
        Dictionary<string, string> values,
        string? token)
    {
        values[resource.IdField] = id;

        return new ResourceEditViewModel
        {
            Resource = resource,
            Id = id,
            Values = values,
            LookupOptions = await BuildLookupOptions(resource, token, includeFilterFields: false)
        };
    }

    public Dictionary<string, object?> BuildCreatePayload(
        ApiResourceDefinition resource,
        IFormCollection form)
    {
        var payload = new Dictionary<string, object?>();

        foreach (var field in resource.Fields)
        {
            if (field.ReadOnly || !field.IncludeInCreatePayload)
            {
                continue;
            }

            var value = form[field.Name].ToString();
            if (!field.ShowInCreate && string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            payload[field.Name] = ConvertValue(field, value);
        }

        return payload;
    }

    public Dictionary<string, object?> BuildEditPayload(
        ApiResourceDefinition resource,
        string id,
        IFormCollection form)
    {
        var payload = new Dictionary<string, object?>();

        foreach (var field in resource.Fields)
        {
            if (field.ReadOnly || !field.IncludeInEditPayload)
            {
                continue;
            }

            var value = form[field.Name].ToString();
            if (string.IsNullOrWhiteSpace(value) && field.Name == resource.IdField)
            {
                value = id;
            }

            payload[field.Name] = ConvertValue(field, value);
        }

        return payload;
    }

    public ResourceFilter? FindFilter(ApiResourceDefinition resource, string filterKey)
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

    public string BuildFilterEndpoint(ResourceFilter filter, IFormCollection form)
    {
        var endpoint = filter.EndpointTemplate;

        foreach (var field in filter.Fields)
        {
            var value = form[field.Name].ToString();
            endpoint = endpoint.Replace(
                "{" + field.Name + "}",
                Uri.EscapeDataString(value),
                StringComparison.OrdinalIgnoreCase);
        }

        return endpoint;
    }

    public Dictionary<string, string> ReadValuesFromJson(JsonElement row, ApiResourceDefinition resource)
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

    public Dictionary<string, string> ReadValuesFromForm(
        ApiResourceDefinition resource,
        IFormCollection form)
    {
        var values = new Dictionary<string, string>();

        foreach (var field in resource.Fields)
        {
            if (field.ReadOnly || !field.IncludeInEditPayload)
            {
                continue;
            }

            values[field.Name] = form[field.Name].ToString();
        }

        return values;
    }

    private async Task<Dictionary<string, List<LookupOption>>> BuildLookupOptions(
        ApiResourceDefinition resource,
        string? token,
        bool includeFilterFields)
    {
        var lookupKeys = GetLookupKeys(resource, includeFilterFields);
        var options = new Dictionary<string, List<LookupOption>>(StringComparer.OrdinalIgnoreCase);

        foreach (var lookupKey in lookupKeys)
        {
            options[lookupKey] = await GetLookupOptions(lookupKey, token);
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

    private async Task<List<LookupOption>> GetLookupOptions(string lookupKey, string? token)
    {
        var endpoint = GetLookupEndpoint(lookupKey);
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return [];
        }

        var result = await _apiClient.GetAsync(endpoint, token);
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

    private static bool RoleIs(string currentRole, params string[] roles)
    {
        foreach (var role in roles)
        {
            if (string.Equals(role, currentRole, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
