using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.Shared.ValueObjects;

public record ProductId
{
    public Guid Value { get; }

    public ProductId(Guid value)
    {
        if (value == Guid.Empty) throw new InvalidProductIdException();
        Value = value;
    }
    
    public static implicit operator Guid(ProductId id) => id.Value;
    public static implicit operator ProductId(Guid id) => new(id);
    
}