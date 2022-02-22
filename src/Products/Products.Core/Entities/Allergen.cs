using IGroceryStore.Shared.Abstraction.Common;

namespace IGroceryStore.Products.Core.Entities;

public class Allergen : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
}