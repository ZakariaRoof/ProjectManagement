using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementApp.Services;

namespace ProjectManagementApp.Controllers;

[Authorize]
public class SettingsController : Controller
{
    private readonly IAuthService _authService;

    public SettingsController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.CurrentUser = await _authService.GetCurrentUserAsync();
        return View();
    }
}
