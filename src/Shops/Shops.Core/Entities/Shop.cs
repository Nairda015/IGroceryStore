using IGroceryStore.Shops.Core.Common;
using IGroceryStore.Shops.Core.ValueObjects;

namespace IGroceryStore.Shops.Core.Entities;

public class Shop
{
    public ulong Id { get; set; }
    public ulong ShopChainId { get; set; }
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public List<Adress> Adresses { get; set; }
    public Rating Rating { get; set; }
}

