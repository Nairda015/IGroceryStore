using IGroceryStore.Users.Core.Exceptions;

namespace IGroceryStore.Users.Core.ValueObjects;

public record Email
{
    public string Value { get; }
    
    public Email(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidEmailException(value);
        }
        Value = value;
    }
    
    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string value) => new(value);
}