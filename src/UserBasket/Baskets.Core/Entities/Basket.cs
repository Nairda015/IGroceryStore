using IGroceryStore.Baskets.Core.ValueObjects;

namespace IGroceryStore.Baskets.Core.Entities;

internal class Basket
{
    public Guid Id { get; }
    public UserId OwnerId { get; }
    public BasketName Name { get; private set; }
    public ISet<Product> Products { get; private set; } = new HashSet<Product>();

    private Basket()
    {
        
    }

    internal Basket(Guid id, UserId ownerId, BasketName name)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
    }
}