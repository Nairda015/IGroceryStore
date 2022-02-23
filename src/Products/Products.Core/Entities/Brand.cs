using IGroceryStore.Products.Core.ValueObjects;

namespace IGroceryStore.Products.Core.Entities;

public class Brand
{
    public BrandId Id { get; set; }
    public string Name { get; set; }
}