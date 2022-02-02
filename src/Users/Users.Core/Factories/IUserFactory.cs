using IGroceryStore.Users.Core.Entities;
using IGroceryStore.Users.Core.ValueObjects;

namespace IGroceryStore.Users.Core.Factories;

public interface IUserFactory
{
    User Create(UserId id, FirstName firstName, LastName lastName, Email email, Password password);
}