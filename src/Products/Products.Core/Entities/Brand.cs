using IGroceryStore.Products.Core.ValueObjects;

namespace IGroceryStore.Products.Core.Entities;

internal sealed class Brand
{
    public required BrandId Id { get; init; }
    public required string Name { get; set; }
}