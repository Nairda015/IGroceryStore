using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Shared.ValueObjects;

public class BasketId
{
    public ulong Value { get; }

    public BasketId(ulong value)
    {
        if (value == 0) throw new InvalidBasketIdException();
        Value = value;
    }
    
    public static implicit operator ulong(BasketId id) => id.Value;
    public static implicit operator BasketId(ulong id) => new(id);
}
