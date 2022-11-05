using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace Products.IntegrationTests.Products;

public class DeleteProductsTests : IClassFixture<ProductApiFactory>
{
    private readonly HttpClient _client;
    private readonly ProductApiFactory _apiFactory;

    public DeleteProductsTests(ProductApiFactory productApiFactory)
    {
        _apiFactory = productApiFactory;
        _client = productApiFactory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task CreateProduct_WhenDataIsValid()
    {
        // Arrange id = 228099032
        var createProductRequest = TestProducts.CreateProduct;
        var responseWithProductLocation = 
            await _client.PostAsJsonAsync("products", createProductRequest.Body);
        
        
        responseWithProductLocation.StatusCode.Should().Be(HttpStatusCode.Accepted);

        // Act
        var response = await _client.DeleteAsync($"products/{responseWithProductLocation.Headers.Location}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
