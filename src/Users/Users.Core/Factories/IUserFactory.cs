using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Entities;
using IGroceryStore.Users.ValueObjects;

namespace IGroceryStore.Users.Factories;

public interface IUserFactory
{
    User Create(UserId id, FirstName firstName, LastName lastName, Email email, string password);
}
