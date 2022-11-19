using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using IGroceryStore.Shared.Tests.Auth;
using System.Security.Claims;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Products.IntegrationTests;
using IGroceryStore.Products.IntegrationTests.TestModels;

namespace Products.IntegrationTests.Products;

[UsesVerify]
[Collection("ProductCollection")]
public class DeleteProductsTests : IClassFixture<ProductApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;
    private readonly ProductsFakeSeeder _productsFakeSeeder;

    public DeleteProductsTests(ProductApiFactory productApiFactory)
    {
        _client = productApiFactory.HttpClient;
        _resetDatabase = productApiFactory.ResetDatabaseAsync;
        productApiFactory
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

        _productsFakeSeeder = new ProductsFakeSeeder(productApiFactory);
    }

    [Fact]
    public async Task DeleteProduct_WhenProductExists()
    {
        // Arrange
        await _productsFakeSeeder.SeedData();
        var product = TestProducts.Product;

        // Act
        var response = await _client.DeleteAsync($"api/products/{product.Id.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ReturnNotFound_WhenProductNotExists()
    {
        // Arrange
        var product = TestProducts.Product;

        // Act
        var response = await _client.DeleteAsync($"api/products/{product.Id.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => _resetDatabase();
}
