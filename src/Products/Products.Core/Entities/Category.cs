using IGroceryStore.Products.Core.ValueObjects;

namespace IGroceryStore.Products.Core.Entities;

public class Category
{
    public CategoryId Id { get; set; }
    public string Name { get; set; }
}