using System.Security.Claims;

namespace IGroceryStore.Shared.Tests.Auth
{
    public interface IMockUser
    {
        List<Claim> Claims { get; }
    }
    public class MockUser : IMockUser
    {
        public List<Claim> Claims { get; private set; } = new();
        public MockUser(params Claim[] claims)
            => Claims = claims.ToList();
    }
}
