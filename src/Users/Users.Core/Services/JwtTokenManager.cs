using System.Security.Cryptography;
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
            .AddClaim(Shared.Abstraction.Constants.Claims.Name.Expire, DateTimeOffset.UtcNow.AddSeconds(_settings.ExpireSeconds).ToUnixTimeSeconds())
            .AddClaim(Shared.Abstraction.Constants.Claims.Name.UserId, user.Id.Value)
            .Issuer(_settings.Issuer)
            .Audience(Shared.Abstraction.Constants.Tokens.Audience.Access)
            .Encode();
    }
    
    public IDictionary<string, object> VerifyToken(string token)
    {
        return new JwtBuilder()
            .WithSecret(_settings.Key)
            .MustVerifySignature()
            .Decode<IDictionary<string, object>>(token);
    }
    
    public (string refreshToken, string jwt) GenerateRefreshToken(User user)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create()){
            rng.GetBytes(randomNumber);
            Convert.ToBase64String(randomNumber);
        }

        var refreshToken = Encoding.ASCII.GetString(randomNumber);

        var jwt = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(_settings.Key)
            .AddClaim(Shared.Abstraction.Constants.Claims.Name.Expire, DateTimeOffset.UtcNow.AddHours(4).ToUnixTimeSeconds())
            .AddClaim(Shared.Abstraction.Constants.Claims.Name.RefreshToken, refreshToken)
            .AddClaim(Shared.Abstraction.Constants.Claims.Name.UserId, user.Id.Value)
            .Issuer(_settings.Issuer)
            .Audience(Shared.Abstraction.Constants.Tokens.Audience.Refresh)
            .Encode();

        return (refreshToken, jwt);
    }
}