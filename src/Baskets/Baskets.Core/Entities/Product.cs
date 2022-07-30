using IGroceryStore.Baskets.Core.ValueObjects;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Baskets.Core.Entities;

internal class Product //from the product module
{
    private Product() { }
    

    internal Product(ProductId id, ProductName name, string category)
    {
        Id = id;
        Name = name;
        Category = category;
    }
    public ProductId Id { get; }
    public ProductName Name { get; private set; }
    
    public string Category { get; private set; }
    
    internal void UpdateName(ProductName name)
    {
        Name = name;
    }
    
    internal void UpdateCategory(string category)
    {
        Category = category;
    }
}

internal class RatedProduct : Product // in user basket
{
    internal RatedProduct(ProductId id, ProductName name, PersonalRating rating, string category) : base(id, name, category)
    {
        Rating = rating;
    }

    public PersonalRating? Rating { get; private set; }
    
    internal void UpdatePersonalRating(PersonalRating personalRating)
    {
        Rating = personalRating;
    }
}