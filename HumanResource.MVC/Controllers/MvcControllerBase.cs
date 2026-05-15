using HumanResource.MVC.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HumanResource.MVC.Controllers;

public abstract class MvcControllerBase : Controller
{
    protected string? Token      => HttpContext.Session.GetString(SessionKeys.JwtToken);
    protected string  Role       => HttpContext.Session.GetString(SessionKeys.UserRole)  ?? string.Empty;
    protected string  Email      => HttpContext.Session.GetString(SessionKeys.UserEmail) ?? string.Empty;
    protected string  EmployeeId => HttpContext.Session.GetString(SessionKeys.EmployeeId) ?? string.Empty;

    // True only when a token exists and has not yet expired.
    protected bool IsSignedIn => !string.IsNullOrWhiteSpace(Token) && !IsSessionExpired;

    private bool IsSessionExpired
    {
        get
        {
            var raw = HttpContext.Session.GetString(SessionKeys.TokenExpiration);
            return raw is not null
                && DateTime.TryParse(raw, out var expires)
                && expires <= DateTime.UtcNow;
        }
    }

    // Returns a redirect when the user is not signed in; clears stale session on expiry.
    protected IActionResult? RequireLogin()
    {
        if (IsSignedIn) return null;
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }

    protected IActionResult? RequireRole(params string[] roles)
    {
        var guard = RequireLogin();
        if (guard is not null) return guard;

        return roles.Any(r => string.Equals(r, Role, StringComparison.OrdinalIgnoreCase))
            ? null
            : RedirectToAction("AccessDenied", "Account");
    }

    protected bool RoleIs(params string[] roles)
        => roles.Any(r => string.Equals(r, Role, StringComparison.OrdinalIgnoreCase));

    // Unpacks a JsonElement (array, paginated object, or single item) into a flat list of rows.
    protected static IReadOnlyList<JsonElement> ExtractRows(JsonElement? data)
    {
        if (data is null) return [];

        var root = data.Value;

        if (root.ValueKind == JsonValueKind.Array)
            return root.EnumerateArray().Select(e => e.Clone()).ToList();

        if (root.ValueKind == JsonValueKind.Object
            && root.TryGetProperty("data", out var page)
            && page.ValueKind == JsonValueKind.Array)
        {
            return page.EnumerateArray().Select(e => e.Clone()).ToList();
        }

        return [root.Clone()];
    }
}
