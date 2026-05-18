using HumanResource.MVC.Controllers;
using HumanResource.MVC.Models.Auth;
using HumanResource.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace HumanResource.MVC.Views.Departments;

public class AccountController : MvcControllerBase
{
    private readonly HrApiClient _apiClient;

    public AccountController(HrApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (IsSignedIn)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["AuthScreen"] = true;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewData["AuthScreen"] = true;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _apiClient.PostAsync("api/Auth/login", model, null);
        if (!result.Succeeded || result.Data is null)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Login failed.");
            return View(model);
        }

        var data = result.Data.Value;
        var token = Get(data, "token");
        HttpContext.Session.SetString("JwtToken", token);
        HttpContext.Session.SetString("UserEmail", Get(data, "email"));
        HttpContext.Session.SetString("UserRole", Get(data, "role"));
        HttpContext.Session.SetString("TokenExpiration", Get(data, "expiration"));
        HttpContext.Session.SetString("EmployeeId", ReadEmployeeId(token));

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

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _apiClient.PostAsync("api/Auth/register", model, null);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Registration failed.");
            return View(model);
        }

        TempData["Notice"] = "Account created. Sign in with your new credentials.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        ViewData["AuthScreen"] = !IsSignedIn;
        return View();
    }

    private static string Get(System.Text.Json.JsonElement data, string propertyName)
    {
        return data.TryGetProperty(propertyName, out var value) ? value.ToString() : string.Empty;
    }

    private static string ReadEmployeeId(string token)
    {
        var parts = token.Split('.');
        if (parts.Length < 2)
        {
            return string.Empty;
        }

        try
        {
            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));

            using var document = JsonDocument.Parse(json);
            return document.RootElement.TryGetProperty("EmployeeId", out var employeeId)
                ? employeeId.ToString()
                : string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}
