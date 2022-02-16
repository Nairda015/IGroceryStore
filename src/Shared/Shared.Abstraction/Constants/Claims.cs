using System.Security.Claims;

namespace IGroceryStore.Shared.Abstraction.Constants;

public static class Claims
{
    public static class Name
    {
        public const string UserId  = "user-id";
        public const string Role  = ClaimTypes.Role;
        public const string RefreshToken  = "refresh-token";
        public const string Expire  = "expire";
    }
    
    
}

public static class Tokens
{
    public static class Audience
    {
        public const string Access  = "access";
        public const string Refresh  = "refresh";
    }
}