using IGroceryStore.Baskets.Core.Entities;

namespace IGroceryStore.Baskets.Core.Persistence;

public static class BasketDbSeed
{
    public static async Task SeedSampleDataAsync(this BasketDbContext context)
    {
        // Seed, if necessary
        var basket = new Basket(Guid.NewGuid(), new Guid(1,3,5,7,9,2,4,6,8,0,0), "AdminBasket");
        context.Baskets.Add(basket);
        await context.SaveChangesAsync();
    }
}