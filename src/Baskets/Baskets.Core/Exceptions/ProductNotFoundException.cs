using IGroceryStore.Shared.Abstraction.Exceptions;

namespace IGroceryStore.Baskets.Exceptions;

public class ProductNotFoundException : GroceryConsumerException
{
    public ulong ProductId { get; }

    public ProductNotFoundException(bool shouldBeRetried, Guid? correlationId, ulong productId, string message)
        : base(shouldBeRetried, correlationId, message)
        => ProductId = productId;
}
