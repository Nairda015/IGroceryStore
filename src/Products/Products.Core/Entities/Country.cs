using IGroceryStore.Products.ValueObjects;

namespace IGroceryStore.Products.Entities;

internal sealed class Country
{
    public required CountryId Id { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
}
