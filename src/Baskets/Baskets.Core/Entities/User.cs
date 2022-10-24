using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Baskets.Core.Entities;

internal record User
{
    public required UserId Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    
    public ISet<Guid> Baskets { get; init; } = new HashSet<Guid>();
}
