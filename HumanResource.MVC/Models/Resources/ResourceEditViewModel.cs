namespace HumanResource.MVC.Models.Resources;

public class ResourceEditViewModel
{
    public ApiResourceDefinition Resource { get; set; } = new();

    public string Id { get; set; } = string.Empty;

    public Dictionary<string, string> Values { get; set; } = [];

    public Dictionary<string, List<LookupOption>> LookupOptions { get; set; } = [];

    public string? Error { get; set; }
}
