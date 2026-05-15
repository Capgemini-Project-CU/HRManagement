namespace HumanResource.MVC.Models.Config;

/// <summary>
/// Strongly-typed representation of the "ApiSettings" section in appsettings.json.
/// Injected via IOptions&lt;ApiSettings&gt; wherever the API base URL is needed.
/// </summary>
public class ApiSettings
{
    public const string SectionName = "ApiSettings";

    public string BaseUrl { get; set; } = "http://localhost:5032/";
}
