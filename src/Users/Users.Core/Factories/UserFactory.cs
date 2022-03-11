using IGroceryStore.Shared.Services;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Core.Entities;
using IGroceryStore.Users.Core.ValueObjects;

namespace IGroceryStore.Users.Core.Factories;

public class UserFactory : IUserFactory
{
    public User Create(UserId id, FirstName firstName, LastName lastName, Email email, string password)
        => new(id, firstName, lastName, email, HashingService.HashPassword(password));
}