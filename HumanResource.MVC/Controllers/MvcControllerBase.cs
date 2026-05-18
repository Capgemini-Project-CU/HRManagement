using Microsoft.AspNetCore.Mvc;

namespace HumanResource.MVC.Controllers;

public abstract class MvcControllerBase : Controller
{
    protected string? Token => HttpContext.Session.GetString("JwtToken");

    protected string Role => HttpContext.Session.GetString("UserRole") ?? string.Empty;

    protected string Email => HttpContext.Session.GetString("UserEmail") ?? string.Empty;

    protected string EmployeeId => HttpContext.Session.GetString("EmployeeId") ?? string.Empty;

    protected bool IsSignedIn => !string.IsNullOrWhiteSpace(Token);

    protected IActionResult? RequireLogin()
    {
        return IsSignedIn ? null : RedirectToAction("Login", "Account");
    }

    protected IActionResult? RequireRole(params string[] roles)
    {
        if (!IsSignedIn)
        {
            return RedirectToAction("Login", "Account");
        }

        return roles.Any(role => string.Equals(role, Role, StringComparison.OrdinalIgnoreCase))
            ? null
            : RedirectToAction("AccessDenied", "Account");
    }
}
