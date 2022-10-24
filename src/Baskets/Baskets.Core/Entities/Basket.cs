using IGroceryStore.Baskets.Core.ValueObjects;
using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Baskets.Core.Entities;

internal class Basket
{
    //private public maybe can share to friends?
    public required BasketId Id { get; init; }
    public required UserId OwnerId { get; init; }
    public required BasketName Name { get; init; }

    public ISet<ProductId> Products { get; set; } = new HashSet<ProductId>();
}
