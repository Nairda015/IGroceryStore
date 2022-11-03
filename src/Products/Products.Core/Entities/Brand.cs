using IGroceryStore.Products.ValueObjects;

namespace IGroceryStore.Products.Entities;

internal sealed class Brand
{
    public required BrandId Id { get; init; }
    public required string Name { get; set; }
}
