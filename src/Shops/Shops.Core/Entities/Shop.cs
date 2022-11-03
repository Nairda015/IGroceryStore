using IGroceryStore.Shops.Common;
using IGroceryStore.Shops.ValueObjects;

namespace IGroceryStore.Shops.Entities;

public class Shop
{
    public ulong Id { get; set; }
    public ulong ShopChainId { get; set; }
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public List<Adress> Adresses { get; set; }
    public Rating Rating { get; set; }
}

