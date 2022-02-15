using IGroceryStore.Users.Core.Entities;

namespace IGroceryStore.Users.Core.Services;

public interface ITokenManager
{
    string GenerateAccessToken(User user);
    IDictionary<string, object> VerifyToken(string token);
}