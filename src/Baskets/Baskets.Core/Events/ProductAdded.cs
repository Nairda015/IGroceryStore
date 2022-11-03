namespace IGroceryStore.Baskets.Events;

public record ProductAdded(ulong ProductId, string Name, string Category);
