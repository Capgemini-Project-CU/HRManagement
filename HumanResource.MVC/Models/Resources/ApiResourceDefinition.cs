namespace HumanResource.MVC.Models.Resources;

public class ApiResourceDefinition
{
    public string Key { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;

    public string IdField { get; set; } = string.Empty;

    public string Icon { get; set; } = "bi-grid";

    public string Summary { get; set; } = string.Empty;

    public string[] ViewRoles { get; set; } = [];

    public string[] CreateRoles { get; set; } = [];

    public string[] EditRoles { get; set; } = [];

    public string[] DeleteRoles { get; set; } = [];

    public List<ApiField> Fields { get; set; } = [];

    public List<ResourceFilter> Filters { get; set; } = [];

    public bool ShowDetails { get; set; }
}

public class ResourceFilter
{
    public string Key { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string EndpointTemplate { get; set; } = string.Empty;

    public string[] Roles { get; set; } = [];

    public List<ApiField> Fields { get; set; } = [];
}
