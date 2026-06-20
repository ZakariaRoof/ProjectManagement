using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementApp.Models;
using ProjectManagementApp.Services;

namespace ProjectManagementApp.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly ILocalizationService _localization;

    public AccountController(IAuthService authService, ILocalizationService localization)
    {
        _authService = authService;
        _localization = localization;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!await _authService.SignInAsync(model.EmployeeCode, model.Password, model.RememberMe))
        {
            model.ErrorMessage = _localization.IsRtl()
                ? "بيانات تسجيل الدخول غير صحيحة. يرجى المحاولة مرة أخرى."
                : "Invalid login credentials. Please try again.";

            return View(model);
        }
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("ApprovedProjects", "MyProjects");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _authService.GetCurrentUserAsync();
        if (user == null)
            return RedirectToAction(nameof(Login));

        return View(user);
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangeLanguage(string language, string? returnUrl = null)
    {
        var culture = string.Equals(language, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "ar";
        _localization.SetLanguage(culture);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("ApprovedProjects", "MyProjects");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
