using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Tests.Auth;
using IGroceryStore.Users.ReadModels;
using Microsoft.AspNetCore.TestHost;

namespace IGroceryStore.Users.IntegrationTests.Users;

[UsesVerify]
[Collection("UserCollection")]
public class GetUserTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;

    public GetUserTests(UserApiFactory apiFactory)
    {
        _client = apiFactory.HttpClient;
        _resetDatabase = apiFactory.ResetDatabaseAsync;
        apiFactory
            .WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services.RegisterUser(new[]
                    {
                        new Claim(Constants.Claims.Name.UserId, "1"),
                        new Claim(Constants.Claims.Name.Expire,
                            DateTimeOffset.UtcNow.AddSeconds(2137).ToUnixTimeSeconds().ToString())
                    });
                })); // override authorized user;
    }

    [Fact]
    public async Task GetUser_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var registerRequest = TestUsers.Register;
        var responseWithUserLocation = await _client.PostAsJsonAsync("api/users/register", registerRequest.Body);
        responseWithUserLocation.StatusCode.Should().Be(HttpStatusCode.Accepted);

        // Act
        var response = await _client.GetAsync(responseWithUserLocation.Headers.Location);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserReadModel>();
        user.Should().NotBeNull();

        response.RequestMessage!.RequestUri.Should().Be($"http://localhost/api/users/{user!.Id}");
        // TODO: Something is wrong with scrubber
        response.RequestMessage!.RequestUri = new Uri(response.RequestMessage.RequestUri!
            .ToString()
            .Replace($"{user.Id}", "Guid_1"));
        await Verify(new { response, user });
    }
    
    
    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => _resetDatabase();
}
