using IGroceryStore.Stores.Core.Common;

namespace IGroceryStore.Stores.Core.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int TrustLevel { get; set; }
    public Badge Badge { get; set; }
}