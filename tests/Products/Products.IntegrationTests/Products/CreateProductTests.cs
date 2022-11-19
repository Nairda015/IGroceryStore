using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Bogus;
using FluentAssertions;
using IGroceryStore.Products.Features.Products.Commands;
using IGroceryStore.Products.IntegrationTests.TestModels;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Products.ValueObjects;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Tests.Auth;
using Microsoft.AspNetCore.TestHost;
using Products.IntegrationTests;

namespace IGroceryStore.Products.IntegrationTests.Products;

[UsesVerify]
[Collection("ProductCollection")]
public class CreateProductTests : IClassFixture<ProductApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDatabase;
    private readonly ProductsFakeSeeder _productsFakeSeeder;

    public CreateProductTests(ProductApiFactory productApiFactory)
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
        //productApiFactory.SeedProductDb();
    }

    [Fact]
    public async Task CreateProduct_WhenDataIsValid()
    {
        // Arrange
        await _productsFakeSeeder.SeedData();
        var createProduct = GetFakeCreateProduct();

        // Act
        var response = await _client.PostAsJsonAsync($"api/products", createProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task CreateProduct_WhenCategoryNotExists_ShouldReturnNotFound()
    {
        // Arrange
        await _productsFakeSeeder.SeedData();
        var createProduct = GetFakeCreateProduct();

        await _client.DeleteAsync($"api/categories/{createProduct.CategoryId}");

        // Act
        var response = await _client.PostAsJsonAsync($"api/products", createProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private CreateProduct.CreateProductBody GetFakeCreateProduct()
    {
        var units = new[] { Unit.Centimeter, Unit.Gram, Unit.Milliliter, Unit.Piece };

        var brandsIds = TestBrands.Brands
            .Select(x => x.Id.Id)
            .ToList();

        var countriesIds = TestCountries.Countries
            .Select(x => x.Id.Id)
            .ToList();

        var categoriesIds = TestCategories.Categories
            .Select(x => x.Id.Id)
            .ToList();

        var faker = new Faker();

        var body = new CreateProduct.CreateProductBody(
            faker.Commerce.ProductName(),
            new QuantityReadModel(faker.Random.UInt(1, 20) * 100, faker.PickRandom(units)),
            faker.PickRandom(brandsIds),
            faker.PickRandom(countriesIds),
            faker.PickRandom(categoriesIds)
            );

        return body;
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => _resetDatabase();
}
