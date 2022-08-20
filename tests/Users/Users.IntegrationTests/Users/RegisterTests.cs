using System.Net.Http.Json;
using IGroceryStore.Users.Core.Features.Users;
using Bogus;
using FluentAssertions;
using IGroceryStore.Shared.ValueObjects;
using IGroceryStore.Users.Core.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Users.IntegrationTests.Users;

[UsesVerify]
public class RegisterTests : IClassFixture<UserApiFactory>
{
    private readonly HttpClient _client;
    private readonly UserApiFactory _apiFactory;
    private readonly Faker<Register> _userGenerator = new RegisterFaker();

    public RegisterTests(UserApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _client = apiFactory.CreateClient();
    }

    [Fact]
    public async Task Create_CreatesUser_WhenDataIsValid()
    {
        // Arrange
        var registerRequest = _userGenerator.Generate();

        // Act
        var response = await _client.PostAsJsonAsync("users/register", registerRequest.Body);
        
        // Assert
        await Verify(response);
        
        //Cleanup
        var path = response.Headers.Location;
        var id = Guid.Parse(path!.Segments.Last());

        await RemoveUserById(id);
    }

    private async Task RemoveUserById(Guid id)
    {
        using var scope = _apiFactory.Services.CreateScope();
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