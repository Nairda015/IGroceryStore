using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Persistence.Contexts;

namespace IGroceryStore.Products.Core.Persistence.Seeders;

public static class ProductsDbSeed
{
    public static async Task SeedSampleDataAsync(this ProductsDbContext context)
    {
        if (context.Products.Any()) return;

        var id = new Guid(1, 3, 5, 7, 9, 2, 4, 6, 8, 0, 1);

        //CosmosDb
        var product = new Product()
        {
            Id = Guid.NewGuid(),
            Name = "TEST",
            Brand = "TEST",
        };
        context.Products.Add(product);
        await context.SaveChangesAsync();
    }
}