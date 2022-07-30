using IGroceryStore.Shops.Core.Common;
using IGroceryStore.Shops.Core.ValueObjects;

namespace IGroceryStore.Shops.Core.Entities;

public class Store
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Adress> Adresses { get; set; }
    public Rating Rating { get; set; }
}

