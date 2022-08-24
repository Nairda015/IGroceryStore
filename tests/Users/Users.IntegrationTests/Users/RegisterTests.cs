using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Users.IntegrationTests.Users;

[UsesVerify]
public class RegisterTests : IClassFixture<UserApiFactory>
{
    private readonly HttpClient _client;
    private readonly UserApiFactory _apiFactory;
    //TODO: Add check for harness
    private readonly ITestHarness _testHarness;

    public RegisterTests(UserApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _testHarness = apiFactory.Services.GetTestHarness();
        _client = apiFactory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Register_CreatesUser_WhenDataIsValid()
    {
        // Arrange
        var registerRequest = TestUsers.Register;

        // Act
        var response = await _client.PostAsJsonAsync("users/register", registerRequest.Body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        await Verify(response);
        
        //Cleanup
        var path = response.Headers.Location;
        var id = Guid.Parse(path!.Segments.Last());
        await _apiFactory.RemoveUserById(id);
    }
}