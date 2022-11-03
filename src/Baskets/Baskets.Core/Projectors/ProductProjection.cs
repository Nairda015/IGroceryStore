using IGroceryStore.Baskets.ReadModels;

namespace IGroceryStore.Baskets.Projectors;

public record ProductProjectionForShop
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    public List<Price> Prices { get; } = new();
}
