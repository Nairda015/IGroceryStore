using IGroceryStore.Shared.ValueObjects;

namespace IGroceryStore.Baskets.Core.Entities;

internal class User 
{
    private User()
    {
        
    }

    internal User(UserId id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }
    
    public UserId Id { get; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public List<Basket> Baskets { get; private set; }

}