using IGroceryStore.Users.Entities;
using IGroceryStore.Users.ValueObjects;

namespace IGroceryStore.Users.Persistence.Mongo.DbModels;

internal record UserDbModel
{
    public required string Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PasswordHash { get; init; }
    
    public required LoginMetadataDbModel LoginMetadata { get; init; } = new();
    public Preferences Preferences { get; init; } = new();
    public TokenStore TokenStore { get; init; } = new();
    public SecurityMetadata SecurityMetadata { get; init; } = new();
}

internal class LoginMetadataDbModel
{
    public ushort AccessFailedCount { get; init; }
    public DateTime? LockoutEnd { get; init; }
}



