using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Products.Core.Entities;

internal class Product : AuditableEntity
{
    public Product()
    {
    }

    public Product(ProductId id,
        ProductName name,
        Description description,
        Quantity quantity,
        BrandId brandId,
        CountryId countryId,
        CategoryId categoryId)
    {
        Id = id;
        Name = name;
        Description = description;
        Quantity = quantity;
        BrandId = brandId;
        CategoryId = categoryId;
        CountryId = countryId;
        IsObsolete = false;
        Allergens = new List<Allergen>();
    }

    public ProductId Id { get; set; }
    public ProductName Name { get; set; }
    public Description Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public BarCode? BarCode { get; set; }
    public Quantity Quantity { get; set; }
    internal bool IsObsolete { get; private set; }
    
    public CountryId CountryId { get; private set; }
    public Country Country { get; set; }
    public BrandId BrandId {get; private set;}
    public Brand Brand { get; set; }
    public CategoryId CategoryId { get; private set; }
    public Category Category { get; set; }
    public List<Allergen> Allergens { get; private set; }
    
    public void MarkAsObsolete()
    {
        IsObsolete = true;
    }
    
    public void AddAllergen(Allergen allergen)
    {
        if(Allergens is null) Allergens = new List<Allergen>();
        else if (Allergens.Contains(allergen)) return;
        
        Allergens.Add(allergen);
    }
}