using IGroceryStore.Users.Exceptions;

namespace IGroceryStore.Users.ValueObjects;

internal sealed record PasswordHash(string Value)
{
    public string Value { get; } = Value ?? throw new InvalidPasswordException();

    public static implicit operator string(PasswordHash passwordHash) => passwordHash.Value;
    public static implicit operator PasswordHash(string value) => new(value);
}
