using System.Net;
using System.Text.Json;

namespace ProjectManagementApp.Services;

public interface ILocalizationService
{
    string GetString(string key);
    void SetLanguage(string culture);
    string GetCurrentLanguage();
    bool IsRtl();
}

public class LocalizationService : ILocalizationService
{
    private Dictionary<string, string> _resources = new();
    private string _currentLanguage;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalizationService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
        _currentLanguage = GetLanguageFromCookie() ?? "ar";
        LoadResources(_currentLanguage);
    }

    private string? GetLanguageFromCookie()
        => _httpContextAccessor.HttpContext?.Request.Cookies["Language"];

    private void LoadResources(string culture)
    {
        var localResourcePath = Path.Combine(_env.ContentRootPath, "Resources", $"AdminResources.{culture}.json");
        var sharedResourcePath = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "..", "..", "Resources", $"AdminResources.{culture}.json"));
        var resourcePath = File.Exists(localResourcePath) ? localResourcePath : sharedResourcePath;

        if (File.Exists(resourcePath))
        {
            var json = File.ReadAllText(resourcePath);
            _resources = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }
        else
        {
            _resources = new Dictionary<string, string>();
        }
    }

    public string GetString(string key)
    {
        if (!_resources.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
            return key;

        var decoded = value;
        for (var i = 0; i < 3; i++)
        {
            var next = WebUtility.HtmlDecode(decoded);
            if (string.Equals(next, decoded, StringComparison.Ordinal))
                break;
            decoded = next;
        }

        return decoded;
    }

    public void SetLanguage(string culture)
    {
        _currentLanguage = culture;
        LoadResources(culture);
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("Language", culture, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        });
    }

    public string GetCurrentLanguage() => _currentLanguage;

    public bool IsRtl() => string.Equals(_currentLanguage, "ar", StringComparison.OrdinalIgnoreCase);
}
