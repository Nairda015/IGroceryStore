using IGroceryStore.Shops.Core.Common;

namespace IGroceryStore.Shops.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int TrustLevel { get; set; }
    public Badge Badge { get; set; }
}