using IGroceryStore.Products.ValueObjects;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Products.Entities;

internal class Product : AuditableEntity
{
    public required ProductId Id { get; init; }
    public required ProductName Name { get; set; }
    public required Quantity Quantity { get; set; }
    public Description? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public BarCode? BarCode { get; set; }
    public bool IsObsolete { get; private set; }
    
    public CountryId CountryId { get; set; }
    public Country Country { get; set; }
    public BrandId BrandId {get; set;}
    public Brand Brand { get; set; }
    public CategoryId CategoryId { get; set; }
    public Category Category { get; set; }
    public HashSet<Allergen> Allergens { get; private set; } = new();
    
    public void MarkAsObsolete()
    {
        IsObsolete = true;
    }
    
    public void AddAllergen(Allergen allergen)
    {
        if (Allergens.Contains(allergen)) return;
        Allergens.Add(allergen);
    }
}
