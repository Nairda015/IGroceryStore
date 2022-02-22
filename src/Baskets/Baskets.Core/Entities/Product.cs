using System.Numerics;
using IGroceryStore.Baskets.Core.ValueObjects;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Baskets.Core.Entities;

internal class Product //from the product module
{
    private Product() { }
    

    internal Product(ProductId id, ProductName name)
    {
        Id = id;
        Name = name;
    }
    public ProductId Id { get; }
    public ProductName Name { get; private set; }
    
    internal void UpdateName(ProductName name)
    {
        Name = name;
    }
}

internal class RatedProduct : Product // in user basket
{
    internal RatedProduct(ProductId id, ProductName name, PersonalRating rating) : base(id, name)
    {
        Rating = rating;
    }

    public PersonalRating? Rating { get; private set; }
    
    internal void UpdatePersonalRating(PersonalRating personalRating)
    {
        Rating = personalRating;
    }
}