using IGroceryStore.Users.Core.Exceptions;

namespace IGroceryStore.Users.Core.ValueObjects;

public record PasswordHash
{
    public string Value { get; }
    
    public PasswordHash(string value)
    {
        if (value is null)
        {
            throw new InvalidPasswordException();
        }
        Value = value;
    }
    
    public static implicit operator string(PasswordHash passwordHash) => passwordHash.Value;
    public static implicit operator PasswordHash(string value) => new(value);
}