using IGroceryStore.Products.ValueObjects;
using IGroceryStore.Shared.Common;

namespace IGroceryStore.Products.Entities;

internal sealed class Allergen : AuditableEntity
{
    public required AllergenId Id { get; init; }
    public required string Name { get; set; }
}
