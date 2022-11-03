using IGroceryStore.Users.Entities;

namespace IGroceryStore.Users.Services;

public interface ITokenManager
{
    string GenerateAccessToken(User user);
    IDictionary<string, object> VerifyToken(string token);
    (string refreshToken, string jwt) GenerateRefreshToken(User user);
}
