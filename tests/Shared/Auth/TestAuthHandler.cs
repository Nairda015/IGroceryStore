using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IGroceryStore.Shared.Tests.Auth
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IMockUser _mockAuthUser;

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IMockUser mockAuthUser)
            : base(options, logger, encoder, clock)
        {
            _mockAuthUser = mockAuthUser;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (_mockAuthUser.Claims.Count == 0)
                return Task.FromResult(AuthenticateResult.Fail("Mock auth user not configured."));

            var identity = new ClaimsIdentity(_mockAuthUser.Claims, AuthConstants.Scheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthConstants.Scheme);

            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
        }
    }
}
