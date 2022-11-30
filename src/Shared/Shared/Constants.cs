using System.Security.Claims;

namespace IGroceryStore.Shared;

public static class Constants
{
    public static class TableNames
    {
        public static string Users => "Users";
        public static string Shops => "Shops";
        public static string Products => "Products";
    }
    
    public static class SwaggerTags
    {
        public const string Users  = "Users";
        public const string Shops  = "Shops";
        public const string Products  = "Products";
        public const string Baskets  = "Baskets";
        public const string HealthChecks  = "HealthChecks";
    }
    
    public static class Claims
    {
        public static class Name
        {
            public const string UserId  = "user-id";
            public const string Role  = ClaimTypes.Role;
            public const string RefreshToken  = "refresh-token";
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
}
