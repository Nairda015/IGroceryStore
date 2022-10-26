using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Shared.Auth
{
    public static class AuthServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddTestAuthentication(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(AuthConstants.Scheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services.AddAuthentication(AuthConstants.Scheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(AuthConstants.Scheme, options => { });
        }
    }
}
