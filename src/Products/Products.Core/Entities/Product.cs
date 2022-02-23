using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Products.Core.Entities;

internal class Product : AuditableEntity
{
    public ProductId Id { get; set; }
    public ProductName Name { get; set; }
    public Description Description { get; set; }
    public Uri ImageUrl { get; set; }
    public BarCode BarCode { get; set; }
    public Quantity Quantity { get; set; }
    
    public int CategoryId { get; set; }
    public Country Country { get; set; }
    public List<Allergen> Allergens { get; set; }
    public Brand Brand { get; set; }
    public Category Category { get; set; }
    internal bool IsObsolete { get; private set; }
    
    public void MarkAsObsolete()
    {
        IsObsolete = true;
    }
}