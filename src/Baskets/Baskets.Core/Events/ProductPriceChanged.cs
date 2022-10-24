namespace IGroceryStore.Baskets.Core.Events;

public record ProductPriceChanged(ulong ShopChainId, decimal NewPrice);
