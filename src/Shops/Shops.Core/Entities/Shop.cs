using IGroceryStore.Shops.ValueObjects;

namespace IGroceryStore.Shops.Entities;

public class Shop
{
    public ulong Id { get; set; }
    public ulong ShopChainId { get; set; }
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public Adress Adress { get; set; }
}
