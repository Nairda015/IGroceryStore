using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Shared.Tests.Auth
{
    public static class AuthServiceCollectionExtensions
    {
        public static IServiceCollection AddTestAuthentication(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(AuthConstants.Scheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            services.AddAuthentication(AuthConstants.Scheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(AuthConstants.Scheme, options => { });

            return services;
        }
        public static void RegisterUser(this IServiceCollection services, IEnumerable<Claim> claims)
        {
            var user = new MockUser(claims.ToArray());
            services.AddSingleton<IMockUser>(_ => user);
        }
    }
}
