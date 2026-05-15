using System.Text.Json;

namespace HumanResource.MVC.Models.Resources;

public class ResourcePageViewModel
{
    public ApiResourceDefinition Resource { get; set; } = new();

    public string Role { get; set; } = string.Empty;

    public IReadOnlyList<JsonElement> Records { get; set; } = [];

    public Dictionary<string, List<LookupOption>> LookupOptions { get; set; } = [];

    public string? ActiveFilterTitle { get; set; }

    public string? Notice { get; set; }

    public string? Error { get; set; }

    public bool CanCreate { get; set; }

    public bool CanEdit { get; set; }

    public bool CanDelete { get; set; }
}
