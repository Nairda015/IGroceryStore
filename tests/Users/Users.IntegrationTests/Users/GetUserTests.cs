using System.Net.Http.Json;
using Bogus;
using FluentAssertions;
using IGroceryStore.Users.Core.Features.Users;
using IGroceryStore.Users.Core.ReadModels;

namespace Users.IntegrationTests.Users;

[Collection(nameof(UserCollection))]
[UsesVerify]
public class GetUserTests : IClassFixture<UserApiFactory>
{
    private readonly HttpClient _client;
    private readonly UserApiFactory _apiFactory;
    private readonly Faker<Register> _userGenerator = new RegisterFaker();
    
    public GetUserTests(UserApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _client = apiFactory.CreateClient();
    }

    [Fact]
    public async Task GetUser_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var registerRequest = _userGenerator.Generate();
        var responseWithUserLocation = await _client.PostAsJsonAsync("users/register", registerRequest.Body);

        // Act
        var response = await _client.GetAsync(responseWithUserLocation.Headers.Location);
        var user = await response.Content.ReadFromJsonAsync<UserReadModel>();
        
        // Assert
        user.Should().NotBeNull();
        
        response.RequestMessage!.RequestUri.Should().Be($"http://localhost/users/{user!.Id}");
        // TODO: Something is wrong with scrubber
        response.RequestMessage.RequestUri = new Uri(response.RequestMessage.RequestUri!
            .ToString()
            .Replace($"{user.Id}", "Guid_1"));
        await Verify(new { response, user });
        
        //Cleanup
        await _apiFactory.RemoveUserById(user.Id);
    }
}