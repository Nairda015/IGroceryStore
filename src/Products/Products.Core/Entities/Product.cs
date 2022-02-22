using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Products.Core.Entities;

internal class Product : AuditableEntity
{
    //Data modeling session for NoSql
    public ProductId Id { get; set; }
    public ProductName Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public BarCode BarCode { get; set; }
    public string Brand { get; set; }
    public string Size { get; set; }
    public string Country { get; set; }
    public List<Allergen> Allergens { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    internal bool IsObsolete { get; private set; }
    
    public void MarkAsObsolete()
    {
        IsObsolete = true;
    }
}