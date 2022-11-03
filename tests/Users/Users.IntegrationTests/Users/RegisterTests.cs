using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MassTransit.Testing;

namespace IGroceryStore.Users.IntegrationTests.Users;

[UsesVerify]
[Collection("UserCollection")]
public class RegisterTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;
    //TODO: Add check for harness
    private readonly ITestHarness _testHarness;

    public RegisterTests(UserApiFactory apiFactory)
    {
        _resetDatabase = apiFactory.ResetDatabaseAsync;
        _testHarness = apiFactory.Services.GetTestHarness();
        _client = apiFactory.HttpClient;
    }

    [Fact]
    public async Task Register_CreatesUser_WhenDataIsValid()
    {
        // Arrange
        var registerRequest = TestUsers.Register;

        // Act
        var response = await _client.PostAsJsonAsync("api/users/register", registerRequest.Body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        await Verify(response);
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => _resetDatabase();
}
