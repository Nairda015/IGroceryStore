using IGroceryStore.Baskets.Core.ReadModels;

namespace IGroceryStore.Baskets.Core.Projectors;

public record ProductProjectionForShop
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    public List<Price> Prices { get; } = new();
}
