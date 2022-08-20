using FluentAssertions;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Core.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Users.IntegrationTests;

public static class UserTestHelper
{
    internal static async Task RemoveUserById(this UserApiFactory apiFactory, Guid id)
    {
        using var scope = apiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        var user = await context.Users
            .Where(x => x.Id == new UserId(id))
            .FirstOrDefaultAsync();

        user.Should().NotBeNull();
        if (user is not null)
        {
            context.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}