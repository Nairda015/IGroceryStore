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
    internal bool IsObsolete { get; private set; }
    
    public CountryId CountryId { get; private set; }
    public Country Country { get; set; }
    public BrandId BrandId {get; private set;}
    public Brand Brand { get; set; }
    public CategoryId CategoryId { get; private set; }
    public Category Category { get; set; }
    public List<Allergen> Allergens { get; set; }
    
    public void MarkAsObsolete()
    {
        IsObsolete = true;
    }
}