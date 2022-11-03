using System.Security.Claims;

namespace IGroceryStore.Shared.Abstraction.Services;

public interface ICurrentUserService
{
    ClaimsPrincipal? Principal { get; }
    Guid? UserId { get; }
    string? UserRole { get; }
}
