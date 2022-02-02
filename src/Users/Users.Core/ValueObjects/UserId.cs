using IGroceryStore.Users.Core.Exceptions;

namespace IGroceryStore.Users.Core.ValueObjects;

public record UserId
{
    public UserId()
    {
    }

    public Guid Value { get; }
    
    public UserId(Guid value)
    {
        if (value == Guid.Empty || value == null)
        {
            throw new InvalidUserIdException();
        }
        Value = value;
    }
    
    public static implicit operator Guid(UserId userId) => userId.Value;
    public static implicit operator UserId(Guid value) => new(value);
}