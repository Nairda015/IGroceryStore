using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace IGroceryStore.Shared.Services;

public interface ICurrentUserService
{
    ClaimsPrincipal? Principal { get; }
    Guid? UserId { get; }
    string? UserRole { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;
    public Guid? UserId => Principal?.FindFirstValue(Constants.Claims.Name.UserId).ToGuid();
    public string? UserRole => Principal?.FindFirstValue(Constants.Claims.Name.Role);
}

internal static class Extensions
{
    public static Guid? ToGuid(this string? value)
    {
        return Guid.TryParse(value, out var result) ? result : null;
    }
}
