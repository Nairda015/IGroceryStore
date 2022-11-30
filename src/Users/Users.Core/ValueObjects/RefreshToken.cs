namespace IGroceryStore.Users.ValueObjects;

internal sealed record RefreshToken(string Value, DateTime ExpiresAt, Guid Jti);
