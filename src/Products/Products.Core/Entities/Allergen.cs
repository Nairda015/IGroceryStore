using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Common;

namespace IGroceryStore.Products.Core.Entities;

internal class Allergen : AuditableEntity
{
    public AllergenId Id { get; set; }
    public AllergenName Name { get; set; }
    public AllergenCode Code { get; set; }
}





