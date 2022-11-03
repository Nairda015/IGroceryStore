using IGroceryStore.Products.ValueObjects;

namespace IGroceryStore.Products.Entities;

internal sealed class Category
{
    public required CategoryId Id { get; init; }
    public required string Name { get; set; }
}
