using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;
using IGroceryStore.API;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Tests.Auth;
using IGroceryStore.Users.IntegrationTests;
using IGroceryStore.Users.ReadModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace IGroceryStore.Users.IntegrationTests.Users;

[UsesVerify]
public class GetUserTests : IClassFixture<UserApiFactory>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<IApiMarker> _apiFactory;

    public GetUserTests(UserApiFactory apiFactory)
    {
        _apiFactory = apiFactory
            .WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services.RegisterUser(new[]
                    {
                        new Claim(Claims.Name.UserId, "1"),
                        new Claim(Claims.Name.Expire,
                            DateTimeOffset.UtcNow.AddSeconds(2137).ToUnixTimeSeconds().ToString())
                    });
                })); // override authorized user;
        _client = _apiFactory
            .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
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

        //Cleanup
        await _apiFactory.RemoveUserById(user.Id);
    }
}
