using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Shared.ValueObjects;

public record UserId
{
    public Guid Value { get; }

    public UserId(Guid value)
    {
        if (value == Guid.Empty) throw new InvalidUserIdException();
        Value = value;
    }
    
    public static implicit operator Guid(UserId id) => id.Value;
    public static implicit operator UserId(Guid value) => new(value);
    
}