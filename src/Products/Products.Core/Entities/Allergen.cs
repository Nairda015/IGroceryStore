using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Common;

namespace IGroceryStore.Products.Core.Entities;

internal sealed class Allergen : AuditableEntity
{
    public required AllergenId Id { get; init; }
    public required string Name { get; set; }
}