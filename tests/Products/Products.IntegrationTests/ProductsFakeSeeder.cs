using IGroceryStore.Products.IntegrationTests.TestModels;
using IGroceryStore.Products.Persistence.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Products.IntegrationTests;

namespace IGroceryStore.Products.IntegrationTests;

public class ProductsFakeSeeder  
{
    private ProductApiFactory _productApiFactory;

    public ProductsFakeSeeder(ProductApiFactory productApiFactory) 
    {
        _productApiFactory = productApiFactory;
    }

    public async Task SeedData()
    {
        //await _productApiFactory.ResetDatabaseAsync();

        using var scope = _productApiFactory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

        await context.Countries.AddRangeAsync(TestCountries.Countries);
        await context.Brands.AddRangeAsync(TestBrands.Brands);
        await context.Categories.AddRangeAsync(TestCategories.Categories);
        await context.Products.AddRangeAsync(TestProducts.Products);
        await context.SaveChangesAsync();
    }
}
