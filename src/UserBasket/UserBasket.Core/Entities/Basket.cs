using IGroceryStore.UserBasket.Core.ValueObjects;

namespace IGroceryStore.UserBasket.Core.Entities;

internal class Basket
{
    public Guid Id { get; }
    public UserId OwnerId { get; }
    public string Name { get; private set; }
    public ISet<Product> Products { get; private set; } = new HashSet<Product>();

    private Basket()
    {
        
    }

    internal Basket(Guid id, UserId ownerId, string name)
    {
        Id = id;
        OwnerId = ownerId;
        Name = name;
    }
}