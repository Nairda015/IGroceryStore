using System.Net;
using System.Net.Http.Json;
using IGroceryStore.Users.Core.Features.Users;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Users.IntegrationTests.Users;

[Collection(nameof(UserCollection))]
[UsesVerify]
public class RegisterTests : IClassFixture<UserApiFactory>
{
    private readonly HttpClient _client;
    private readonly UserApiFactory _apiFactory;
    private readonly Faker<Register> _userGenerator = new RegisterFaker();

    public RegisterTests(UserApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _client = apiFactory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Register_CreatesUser_WhenDataIsValid()
    {
        // Arrange
        var registerRequest = _userGenerator.Generate();

        // Act
        var response = await _client.PostAsJsonAsync("users/register", registerRequest.Body);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        await Verify(response);
        
        //Cleanup
        var path = response.Headers.Location;
        var id = Guid.Parse(path!.Segments.Last());
        await _apiFactory.RemoveUserById(id);
    }
}