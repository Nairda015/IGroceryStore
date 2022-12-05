using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using IGroceryStore.Baskets.Features.Baskets;
using IGroceryStore.Baskets.IntegrationTests.Configuration;
using IGroceryStore.Shared.Tests;

namespace IGroceryStore.Baskets.IntegrationTests.Baskets;

[Collection(TestConstants.Collections.Baskets)]
public class AddBasketTests : BasketApiFactory
{
    [Fact]
    public async Task AddBasket_Returns200OK_WhenValidRequestReceived()
    {
        // Arrange
        var commandBody = new AddBasket.AddBasketBody("Test Basket");

        var requestContent = new StringContent(
            JsonSerializer.Serialize(commandBody),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/users/register", commandBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Guid>(responseContent);
        Assert.NotEqual(Guid.Empty, responseObject);
    }
}
