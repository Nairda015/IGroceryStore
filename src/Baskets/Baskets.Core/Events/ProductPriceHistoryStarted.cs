namespace IGroceryStore.Baskets.Events;

public record ProductPriceHistoryStarted(ulong ShopChainId, decimal InitialPrice);
