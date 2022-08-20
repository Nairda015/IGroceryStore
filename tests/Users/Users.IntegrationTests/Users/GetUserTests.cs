using System.Net.Http.Json;
using Bogus;
using IGroceryStore.Users.Core.Features.Users;

namespace Users.IntegrationTests.Users;

public class GetUserTests : IClassFixture<UserApiFactory>
{
    private readonly HttpClient _client;
    private readonly UserApiFactory _apiFactory;

    private readonly Faker<Register> _customerGenerator = new Faker<Register>()
        .RuleFor(x => x.Body.Email, faker => faker.Person.Email)
        .RuleFor(x => x.Body.FirstName, faker => faker.Person.FirstName)
        .RuleFor(x => x.Body.LastName, faker => faker.Person.LastName)
        .RuleFor(x => x.Body.Password, Guid.NewGuid().ToString())
        .RuleFor(x => x.Body.ConfirmPassword, (_, usr) => usr.Body.Password);
    
    public GetUserTests(UserApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _client = apiFactory.CreateClient();
    }

    [Fact]
    public async Task GetUser_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var userRequestModel = _customerGenerator.Generate();
        var response = await _client.PostAsJsonAsync("users/register", userRequestModel);
            //var userId = await response.Content.ReadFromJsonAsync<>();

        // Act
        //var response2 = await _client.GetAsync($"/users/{id}");

        // Assert
        await Verify(response);
    }
    
}