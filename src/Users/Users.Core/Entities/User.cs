using System.Net;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Exceptions;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Core.Exceptions;
using IGroceryStore.Users.Core.Services;
using IGroceryStore.Users.Core.ValueObjects;
using OneOf;

namespace IGroceryStore.Users.Core.Entities;

public class User : AuditableEntity
{
    private User()
    {
    }

    internal User(UserId id,
        FirstName firstName,
        LastName lastName,
        Email email,
        PasswordHash passwordHash)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        _passwordHash = passwordHash;
    }

    private const int MaxLoginTry = 5;
    private PasswordHash _passwordHash;
    private List<RefreshToken> _refreshTokens = new();
    private ushort _accessFailedCount;
    private DateTime _lockoutEnd;
    public UserId Id { get; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Email Email { get; private set; }
    public bool TwoFactorEnabled { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public bool LockoutEnabled { get; private set; }
    private void UpdatePassword(string password, string oldPassword)
    {
        if (!HashingService.ValidatePassword(oldPassword, _passwordHash.Value))
        {
            _accessFailedCount++;
            throw new IncorrectPasswordException();
        }
        _passwordHash = HashingService.HashPassword(password);
    }
    
    private void UpdateEmail(string email)
    {
        Email = email;
    }
    
    private void ConfirmEmail()
    {
        EmailConfirmed = true;
    }

    internal bool EnableTwoTwoFactor()
    {
        TwoFactorEnabled = true;
        throw new NotImplementedException();
        return true;
    }

    private void Lock()
    {
        LockoutEnabled = true;
        _lockoutEnd = DateTime.UtcNow.AddMinutes(5);
    }
    
    private void Unlock()
    {
        LockoutEnabled = false;
        _accessFailedCount = 0;
    }

    private bool TryUnlock()
    {
        if (_lockoutEnd > DateTime.UtcNow) return false;
        Unlock();
        return true;
    }

    internal OneOf<bool, LoggingTriesExceededException> Login(string password)
    {
        if (!TryUnlock()) return new LoggingTriesExceededException(MaxLoginTry);
        
        if (!HashingService.ValidatePassword(password, _passwordHash.Value))
        {
            _accessFailedCount++;
            return false;
        }
        if (_accessFailedCount <= MaxLoginTry) return true;
        Lock();
        return false;
    }

    internal void AddRefreshToken(RefreshToken refreshToken)
    {
        _refreshTokens.Add(refreshToken);
    }

    public bool TokenExist(RefreshToken refreshToken)
        => _refreshTokens.Exists(x => x.Equals(refreshToken));
    public bool TokenExist(string token)
        => _refreshTokens.Exists(x => x.Value == token);
    
    public void UpdateRefreshToken(string oldTokenValue, string newTokenValue)
    {
        var token = _refreshTokens.First(x => x.Value == oldTokenValue);
        var newToken = token with {Value = newTokenValue};
        _refreshTokens.RemoveAll(x => x.Value == oldTokenValue);
        _refreshTokens.Add(newToken);
    }

    public void TryRemoveOldRefreshToken(string userAgent)
    {
        if (!_refreshTokens.Exists(x => x.UserAgent == userAgent)) return;
        _refreshTokens.RemoveAll(x => x.UserAgent == userAgent);
    }
}

internal class LoggingTriesExceededException : GroceryStoreException
{
    public LoggingTriesExceededException(int maxLoginTry) : base("Try again after 5 min")
    {
    }

    public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
}