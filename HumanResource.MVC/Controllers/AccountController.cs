using HumanResource.MVC.Constants;
using HumanResource.MVC.Models.Auth;
using HumanResource.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace HumanResource.MVC.Controllers;

public class AccountController : MvcControllerBase
{
    private readonly HrApiClient _apiClient;
    private readonly ILogger<AccountController> _logger;

    public AccountController(HrApiClient apiClient, ILogger<AccountController> logger)
    {
        _apiClient = apiClient;
        _logger    = logger;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (IsSignedIn) return RedirectToAction("Index", "Home");

        ViewData["AuthScreen"] = true;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewData["AuthScreen"] = true;

        if (!ModelState.IsValid) return View(model);

        var result = await _apiClient.PostAsync("api/Auth/login", model, null);

        if (!result.Succeeded || result.Data is null)
        {
            _logger.LogWarning("Failed login attempt for {Email}", model.Email);
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Login failed.");
            return View(model);
        }

        var data  = result.Data.Value;
        var email = Get(data, "email");
        var role  = Get(data, "role");
        var token = Get(data, "token");

        HttpContext.Session.SetString(SessionKeys.JwtToken,        token);
        HttpContext.Session.SetString(SessionKeys.UserEmail,       email);
        HttpContext.Session.SetString(SessionKeys.UserRole,        role);
        HttpContext.Session.SetString(SessionKeys.TokenExpiration, Get(data, "expiration"));
        HttpContext.Session.SetString(SessionKeys.EmployeeId,      ReadEmployeeId(token));

        _logger.LogInformation("User {Email} signed in with role {Role}", email, role);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewData["AuthScreen"] = true;
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        ViewData["AuthScreen"] = true;

        if (!ModelState.IsValid) return View(model);

        var result = await _apiClient.PostAsync("api/Auth/register", model, null);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Registration failed for {Email}: {Error}", model.Email, result.ErrorMessage);
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Registration failed.");
            return View(model);
        }

        _logger.LogInformation("New employee account registered for {Email}", model.Email);
        TempData["Notice"] = "Account created. Sign in with your new credentials.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        _logger.LogInformation("User {Email} signed out", Email);
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        ViewData["AuthScreen"] = !IsSignedIn;
        return View();
    }

    private static string Get(JsonElement data, string key)
        => data.TryGetProperty(key, out var value) ? value.ToString() : string.Empty;

    // Decodes the JWT payload (Base64Url) to extract EmployeeId without a JWT library.
    private static string ReadEmployeeId(string token)
    {
        var parts = token.Split('.');
        if (parts.Length < 2) return string.Empty;

        try
        {
            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
            using var document = JsonDocument.Parse(json);

            return document.RootElement.TryGetProperty("EmployeeId", out var id)
                ? id.ToString()
                : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}
