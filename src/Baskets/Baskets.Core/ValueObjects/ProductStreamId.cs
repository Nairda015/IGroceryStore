namespace IGroceryStore.Baskets.ValueObjects;

public record ProductStreamId
{
    private readonly string _value;

    public ProductStreamId(ulong productId, ulong shopChainId)
    {
        _value = $"{productId}-{shopChainId}";
    }

    public static implicit operator string(ProductStreamId productStreamId) => productStreamId._value;
    public override string ToString()
    {
        return _value;
    }
}
