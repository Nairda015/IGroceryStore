using System.Diagnostics;
using System.Net;
using IGroceryStore.Shared.Common;
using IGroceryStore.Shared.Exceptions;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Services;
using IGroceryStore.Users.ValueObjects;

namespace IGroceryStore.Users.Entities;

internal sealed class User : AuditableEntity
{
    public User(PasswordHash passwordHash)
    {
        PasswordHash = passwordHash;
    }
    
    public required UserId Id { get; init; }
    public required FirstName FirstName { get; set; }
    public required LastName LastName { get; set; }
    public required Email Email { get; set; }
    public PasswordHash PasswordHash { get; private set; }
    public LoginMetadata LoginMetadata { get; init; } = new();
    public TokenStore TokenStore { get; init; } = new();
    public SecurityMetadata SecurityMetadata { get; init; } = new();
    public Preferences Preferences { get; init; } = new();
    
    public bool UpdatePassword(string password, string oldPassword)
    {
        if (!HashingService.ValidatePassword(oldPassword, PasswordHash.Value)) return false;
        
        PasswordHash = HashingService.HashPassword(password);
        return true;
    }
    public bool IsPasswordCorrect(string password)
    {
        if (IsLocked()) throw new UnreachableException(
            "This should be checked before",
            new LoggingTriesExceededException());
        
        if (HashingService.ValidatePassword(password, PasswordHash.Value)) return true;
        LoginMetadata.ReportLoginFailure();
        return false;
    }
    
    public bool IsLocked() => LoginMetadata.IsLocked;
}

internal class LoggingTriesExceededException : GroceryStoreException
{
    public LoggingTriesExceededException() : base("Try again after 5 min")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
}
