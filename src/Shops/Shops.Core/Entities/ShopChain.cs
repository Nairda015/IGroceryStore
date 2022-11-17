using IGroceryStore.Shops.ValueObjects;

namespace IGroceryStore.Shops.Entities;

public class ShopChain
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public Rating Rating { get; set; }
}
