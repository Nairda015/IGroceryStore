using IGroceryStore.Shops.ValueObjects;

namespace IGroceryStore.Shops.Entities;

internal class User
{
    private User()
    {
    }

    public User(Guid id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }
    
    public Guid Id { get; init; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public uint TrustLevel { get; private set; }
    public Badge Badge => TrustLevel switch
    {
        <= 10 => Badge.None,
        <= 100 => Badge.Bronze,
        <= 1000 => Badge.Silver,
        _ => Badge.Gold
    };

    internal void AddTrustPoints(uint points)
    {
        this.TrustLevel += points;
    }
}
