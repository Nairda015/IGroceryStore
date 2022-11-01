using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Auth
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
