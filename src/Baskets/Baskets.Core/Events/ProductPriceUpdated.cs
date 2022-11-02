namespace IGroceryStore.Baskets.Core.Events;

public record ProductPriceUpdated(ulong ShopChainId, decimal NewPrice);
