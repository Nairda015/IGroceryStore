using System.Security.Claims;

namespace IGroceryStore.Shared.Abstraction.Constants;

public static class Claims
{
    public static class Name
    {
        public const string Id  = ClaimTypes.NameIdentifier;
        public const string Role  = ClaimTypes.Role;
    }
}