using System.Text;
using IGroceryStore.Shared.Options;
using IGroceryStore.Users.Core.Entities;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.Extensions.Options;

namespace IGroceryStore.Users.Core.Services;

public class JwtTokenManager : ITokenManager
{

    private readonly JwtSettings _settings;
    public JwtTokenManager(IOptionsSnapshot<JwtSettings> settings)
    {
        _settings = settings.Value;
    }
    
    public string GenerateAccessToken(User user)
    {
        return new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(Encoding.ASCII.GetBytes(_settings.Key))
            .AddClaim("exp", DateTimeOffset.UtcNow.AddSeconds(_settings.ExpireSeconds).ToUnixTimeSeconds())
            .AddClaim("id", user.Id.Value)
            .Encode();
    }
    
    public IDictionary<string, object> VerifyToken(string token)
    {
        return new JwtBuilder()
            .WithSecret(_settings.Key)
            .MustVerifySignature()
            .Decode<IDictionary<string, object>>(token);
    }
}