namespace HumanResource.MVC.Models.Resources;

public enum ApiFieldType
{
    Text,
    Email,
    Password,
    Number,
    Date
}

public class ApiField
{
    public string Name { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

    public ApiFieldType Type { get; set; } = ApiFieldType.Text;

    public bool Required { get; set; }

    public bool ShowInTable { get; set; } = true;

    public bool ReadOnly { get; set; }

    public bool ShowInCreate { get; set; } = true;

    public bool ShowInEdit { get; set; } = true;

    public bool IncludeInCreatePayload { get; set; } = true;

    public bool IncludeInEditPayload { get; set; } = true;

    public string? LookupKey { get; set; }
}

public class LookupOption
{
    public string Value { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;
}
