namespace IGroceryStore.Shops.Contracts.Events;

public record ProductAddedToShop(ulong ProductId, ulong ShopChainId, decimal InitialPrice);
