using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Shared.ValueObjects;

public record ProductId
{
    public ulong Value { get; }

    public ProductId(ulong value)
    {
        if (value == 0) throw new InvalidProductIdException();
        Value = value;
    }
    
    public static implicit operator ulong(ProductId id) => id.Value;
    public static implicit operator ProductId(ulong id) => new(id);
}
