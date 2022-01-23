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

    public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var id = User?.FindFirst(x => x.Type == Claims.Name.Id)?.Value;
            if (id is null) return null;
            return Guid.Parse(id);
        }
    }

    public string UserRole => User?.FindFirst(x => x.Type == Claims.Name.Role)?.Value;
}
