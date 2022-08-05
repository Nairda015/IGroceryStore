using IGroceryStore.Products.Core.ValueObjects;

namespace IGroceryStore.Products.Core.Entities;

internal sealed class Category
{
    public required CategoryId Id { get; init; }
    public required string Name { get; set; }
}