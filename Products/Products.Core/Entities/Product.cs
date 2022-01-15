using IGroceryStore.Products.Core.ValueObjects;

namespace IGroceryStore.Products.Core.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Category { get; set; }
    public BarCode BarCode { get; set; }
    public string Brand { get; set; }
    public string Size { get; set; }
    public string Country { get; set; }
    public List<Allergen> Allergens { get; set; }
    public bool IsObsolete { get; private set; }
    
    public void MarkAsObsolete()
    {
        IsObsolete = true;
    }
}