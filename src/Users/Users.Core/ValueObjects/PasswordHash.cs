using IGroceryStore.Users.Exceptions;

namespace IGroceryStore.Users.ValueObjects;

internal sealed record PasswordHash
{
    public string Value { get; }
    
    public PasswordHash(string value)
    {
        Value = value ?? throw new InvalidPasswordException();
    }
    
    public static implicit operator string(PasswordHash passwordHash) => passwordHash.Value;
    public static implicit operator PasswordHash(string value) => new(value);
}
