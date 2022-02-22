using IGroceryStore.Baskets.Core.ValueObjects;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Baskets.Core.Entities;

internal class Basket
{
    //private public maybe can share to friends?
    public BasketId Id { get; }
    public UserId OwnerId { get; }
    public BasketName Name { get; private set; }
    
    private ISet<Product> _products = new HashSet<Product>();
    public ISet<Product> Products => _products;

    private Basket()
    {
        
    }

    internal Basket(BasketId id, UserId ownerId, BasketName name)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
    }
    
    internal void AddProduct(Product product)
    {
        _products.Add(product);
    }
    
    internal void RemoveProduct(Product product)
    {
        _products.Remove(product);
    }
    
    internal void UpdateName(BasketName name)
    {
        Name = name;
    } 
}