using System.Security.Claims;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Services;
using Microsoft.AspNetCore.Http;

namespace IGroceryStore.Shared.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;
    public Guid? UserId => Principal?.FindFirstValue(Claims.Name.UserId).ToGuid();
    public string? UserRole => Principal?.FindFirstValue(Claims.Name.Role);
}

internal static class Extensions
{
    public static Guid? ToGuid(this string? value)
    {
        return Guid.TryParse(value, out var result) ? result : null;
    }
}
