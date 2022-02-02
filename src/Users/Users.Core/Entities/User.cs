using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Users.Core.ValueObjects;

namespace IGroceryStore.Users.Core.Entities;

public class User : AuditableEntity
{
    public User()
    {
        
    }

    internal User(UserId id,
        FirstName firstName,
        LastName lastName,
        Email email,
        Password passwordHash)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        _passwordHash = passwordHash;
    }
    
    private Password _passwordHash;
    private ushort _accessFailedCount;
    private DateTime _lockoutEnd;
    public UserId Id { get; }
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Email Email { get; private set; }
    public bool TwoFactorEnabled { get; private set; } = false;
    public bool EmailConfirmed { get; private set; }
    public bool LockoutEnabled { get; private set; }

    //TODO: generate
    public string ConcurrencyStamp { get; private set; }
    public string SecurityStamp { get; private set; }
    private void UpdatePassword(string password)
    {
        //TODO: add hashing
        _passwordHash = password;
        throw new NotImplementedException();
    }
    
    private void UpdateEmail(string email)
    {
        Email = email;
        throw new NotImplementedException();
    }
    
    private void ConfirmEmail()
    {
        EmailConfirmed = true;
        throw new NotImplementedException();
    }

    internal void Lock()
    {
        LockoutEnabled = true;
        _lockoutEnd = DateTime.Now.AddMinutes(5);
    }
    
    internal void Unlock()
    {
        LockoutEnabled = false;
        _accessFailedCount = 0;
    }
    
    internal bool Login(string email, string password)
    {
        throw new NotImplementedException();
        if (Email != email)
        {
            return false;
        }

        if (_passwordHash != password)
        {
            _accessFailedCount++;
            return false;
        }

        if (_accessFailedCount > 5)
        {
            Lock();
            return false;
        }

        return true;
        
    }
}