using IGroceryStore.Users.Exceptions;

namespace IGroceryStore.Users.ValueObjects;

public record LastName
{
    public string Value { get; }
    
    public LastName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidLastNameException(value);
        }
        Value = value;
    }
    
    public static implicit operator string(LastName lastName) => lastName.Value;
    public static implicit operator LastName(string value) => new(value);
    
}
