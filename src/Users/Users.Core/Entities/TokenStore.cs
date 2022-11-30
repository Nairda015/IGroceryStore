using IGroceryStore.Users.ValueObjects;

namespace IGroceryStore.Users.Entities;

internal sealed class TokenStore
{
    public TokenStore() => RemoveOldRefreshTokens();

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private void RemoveOldRefreshTokens()
        => _refreshTokens.RemoveAll(x => x.ExpiresAt <= DateTime.UtcNow);

    internal void AddRefreshToken(RefreshToken refreshToken)
        => _refreshTokens.Add(refreshToken);

    public bool TokenExist(string token)
        => _refreshTokens.Exists(x => token == x.Value);

    public bool IsJtiValid(string token, Guid jti)
        => _refreshTokens.Exists(x => token == x.Value && jti == x.Jti);

    public void RemoveAllRefreshTokens()
        => _refreshTokens.RemoveAll(_ => true);
}
