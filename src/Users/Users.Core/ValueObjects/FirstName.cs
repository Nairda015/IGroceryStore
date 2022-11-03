using IGroceryStore.Users.Exceptions;

namespace IGroceryStore.Users.ValueObjects;

public record FirstName
{
    public string Value { get; }
    
    public FirstName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidFirstNameException(value);
        }
        Value = value;
    }
    
    public static implicit operator string(FirstName lastName) => lastName.Value;
    public static implicit operator FirstName(string value) => new(value);
}
