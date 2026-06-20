using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using ProjectManagementApp.Models;

namespace ProjectManagementApp.Services;

public interface IAuthService
{
    Task<bool> SignInAsync(string employeeCode, string password, bool rememberMe);
    Task SignOutAsync();
    Task<CurrentUser?> GetCurrentUserAsync();
}

public class AuthService : IAuthService
{
    private const string DemoEmployeeCode = "admin";
    private const string DemoPassword = "admin";

    private readonly string _connectionString;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _connectionString = configuration.GetConnectionString("ProjectManagementDatabase")
            ?? throw new InvalidOperationException("ProjectManagementDatabase connection string not found");
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> SignInAsync(string employeeCode, string password, bool rememberMe)
    {
        if (string.Equals(employeeCode?.Trim(), DemoEmployeeCode, StringComparison.OrdinalIgnoreCase)
            && string.Equals(password, DemoPassword, StringComparison.Ordinal))
        {
            await SignInPrincipalAsync(CreateDemoPrincipal(), rememberMe);
            return true;
        }

        var account = await GetAccountAsync(employeeCode ?? string.Empty);
        if (account == null || string.IsNullOrWhiteSpace(account.PasswordHash) || !BCrypt.Net.BCrypt.Verify(password, account.PasswordHash))
            return false;

        var roles = await GetRoleCodesAsync(account.EmployeeId);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.EmployeeId.ToString()),
            new(ClaimTypes.Name, account.EmployeeNameAr),
            new("EmployeeCode", account.EmployeeCode),
            new("EmployeeId", account.EmployeeId.ToString()),
            new("DepartmentCode", account.DepartmentCode?.ToString() ?? string.Empty),
            new("Department", account.DepartmentName ?? string.Empty),
            new("Position", account.PositionName ?? string.Empty)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await SignInPrincipalAsync(principal, rememberMe);

        await UpdateLastLoginAsync(account.EmployeeId);
        return true;
    }

    public Task SignOutAsync()
        => _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    public Task<CurrentUser?> GetCurrentUserAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return Task.FromResult<CurrentUser?>(null);

        _ = int.TryParse(user.FindFirst("EmployeeId")?.Value, out var employeeId);
        _ = int.TryParse(user.FindFirst("DepartmentCode")?.Value, out var departmentCode);
        return Task.FromResult<CurrentUser?>(new CurrentUser
        {
            EmployeeId = employeeId,
            EmployeeCode = user.FindFirst("EmployeeCode")?.Value ?? string.Empty,
            EmployeeName = user.Identity?.Name ?? string.Empty,
            DepartmentCode = departmentCode == 0 ? null : departmentCode,
            DepartmentName = user.FindFirst("Department")?.Value,
            PositionName = user.FindFirst("Position")?.Value
        });
    }

    private async Task SignInPrincipalAsync(ClaimsPrincipal principal, bool rememberMe)
    {
        await _httpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(90),
                AllowRefresh = true
            });
    }

    private static ClaimsPrincipal CreateDemoPrincipal()
    {
        var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, "0"),
    new(ClaimTypes.Name, "مدير النظام"),
    new("EmployeeCode", DemoEmployeeCode),
    new("EmployeeId", "0"),
    new("DepartmentCode", string.Empty),
    new("Department", "إدارة المشاريع"),
    new("Position", "مدير"),
    new(ClaimTypes.Role, "Administrator")
};

        return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    }

    private async Task<EmployeeAccountRecord?> GetAccountAsync(string employeeCode)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(@"
            SELECT TOP 1 EmployeeId, EmployeeCode, EmployeeNameAr, DepartmentCode, DepartmentName, PositionName, PasswordHash
            FROM dbo.EmployeeAccount
            WHERE EmployeeCode = @EmployeeCode AND IsActive = 1", connection);
        command.Parameters.AddWithValue("@EmployeeCode", employeeCode.Trim());

        using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return new EmployeeAccountRecord
        {
            EmployeeId = reader.GetInt32(0),
            EmployeeCode = reader.GetString(1),
            EmployeeNameAr = reader.GetString(2),
            DepartmentCode = reader.IsDBNull(3) ? null : reader.GetInt32(3),
            DepartmentName = reader.IsDBNull(4) ? null : reader.GetString(4),
            PositionName = reader.IsDBNull(5) ? null : reader.GetString(5),
            PasswordHash = reader.IsDBNull(6) ? null : reader.GetString(6)
        };
    }

    private async Task<List<string>> GetRoleCodesAsync(int employeeId)
    {
        var roles = new List<string>();
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(@"
            SELECT r.RoleCode
            FROM dbo.EmployeeAccountRole ear
            INNER JOIN dbo.AppRole r ON r.RoleId = ear.RoleId
            WHERE ear.EmployeeId = @EmployeeId", connection);
        command.Parameters.AddWithValue("@EmployeeId", employeeId);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            roles.Add(reader.GetString(0));

        return roles;
    }

    private async Task UpdateLastLoginAsync(int employeeId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand("UPDATE dbo.EmployeeAccount SET LastLoginOn = SYSDATETIME() WHERE EmployeeId = @EmployeeId", connection);
        command.Parameters.AddWithValue("@EmployeeId", employeeId);
        await command.ExecuteNonQueryAsync();
    }

    private sealed class EmployeeAccountRecord
    {
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeNameAr { get; set; } = string.Empty;
        public int? DepartmentCode { get; set; }
        public string? DepartmentName { get; set; }
        public string? PositionName { get; set; }
        public string? PasswordHash { get; set; }
    }
}
