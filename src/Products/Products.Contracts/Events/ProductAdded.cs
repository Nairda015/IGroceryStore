namespace IGroceryStore.Products.Contracts.Events;

public record ProductAdded(ulong Id, string Name, string Category);