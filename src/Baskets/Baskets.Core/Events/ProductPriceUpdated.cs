namespace IGroceryStore.Baskets.Events;

public record ProductPriceUpdated(ulong ShopChainId, decimal NewPrice);
