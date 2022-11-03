using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Entities;
using IGroceryStore.Users.Services;
using IGroceryStore.Users.ValueObjects;

namespace IGroceryStore.Users.Factories;

public class UserFactory : IUserFactory
{
    public User Create(UserId id, FirstName firstName, LastName lastName, Email email, string password)
        => new(id, firstName, lastName, email, HashingService.HashPassword(password));
}
