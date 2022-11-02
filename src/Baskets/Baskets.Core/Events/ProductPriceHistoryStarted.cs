namespace IGroceryStore.Baskets.Core.Events;

public record ProductPriceHistoryStarted(ulong ShopChainId, decimal InitialPrice);
