namespace IGroceryStore.Baskets.Core.Events;

public record ProductAdded(ulong ProductId, string Name, string Category);
