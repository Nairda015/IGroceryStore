using FluentAssertions;
using IGroceryStore.API;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Users.IntegrationTests;

public static class UserTestHelper
{
    internal static async Task RemoveUserById<T>(this WebApplicationFactory<T> apiFactory, Guid id) where T: class, IApiMarker
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
