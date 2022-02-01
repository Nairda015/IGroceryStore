using IGroceryStore.Users.Core.Exceptions;

namespace IGroceryStore.Users.Core.ValueObjects;

public record Password
{
    public string Value { get; }
    
    public Password(string value)
    {
        if (value is null)
        {
            throw new InvalidPasswordException();
        }
        Value = value;
        throw new NotImplementedException();
    }
    
    public static implicit operator string(Password password) => password.Value;
    public static implicit operator Password(string value) => new(value);
}