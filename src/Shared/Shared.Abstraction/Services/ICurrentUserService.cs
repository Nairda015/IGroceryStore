using System.Security.Claims;

namespace IGroceryStore.Shared.Abstraction.Services;

public interface ICurrentUserService
{
    ClaimsPrincipal User { get; }
    Guid? UserId { get; }
    string UserRole { get; }
}