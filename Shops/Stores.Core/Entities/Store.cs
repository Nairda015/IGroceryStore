using IGroceryStore.Stores.Core.Common;
using IGroceryStore.Stores.Core.ValueObjects;

namespace IGroceryStore.Stores.Core.Entities;

public class Store
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Adress> Adresses { get; set; }
    public Rating Rating { get; set; }
}

