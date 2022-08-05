using IGroceryStore.Products.Core.ValueObjects;

namespace IGroceryStore.Products.Core.Entities;

internal sealed class Country
{
    public required CountryId Id { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
}