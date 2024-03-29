namespace IGroceryStore.Shops.Contracts.Events;

// What about promotions?
public interface IProductPriceChanged
{
    ulong ProductId { get; init; }
    ulong ShopChainId { get; init; }
    decimal NewPrice { get; init; }
    bool IsLowestPrice { get; init; }
}

public record ProductPriceChanged(ulong ProductId, ulong ShopChainId, ulong ShopId, decimal NewPrice, bool IsLowestPrice) : IProductPriceChanged;

public record ProductPriceReported(ulong ProductId);
