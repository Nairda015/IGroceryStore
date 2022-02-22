using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Shared.ValueObjects;

public class BasketId
{
    public Guid Value { get; }

    public BasketId(Guid value)
    {
        if (value == Guid.Empty) throw new InvalidBasketIdException();
        Value = value;
    }
    
    public static implicit operator Guid(BasketId id) => id.Value;
    public static implicit operator BasketId(Guid value) => new(value);
}