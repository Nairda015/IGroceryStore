using IGroceryStore.UserBasket.Core.ValueObjects;
//using Microsoft.

namespace IGroceryStore.UserBasket.Core.Entities;

public class User 
{
    private User()
    {
        
    }

    internal User(UserId id, string firstName, string lastName, string email, string password)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
    }
    
    public Guid Id { get; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
}