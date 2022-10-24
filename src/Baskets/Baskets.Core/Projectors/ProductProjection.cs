using IGroceryStore.Baskets.Core.Events;
using IGroceryStore.Baskets.Core.ReadModels;
using Marten.Events;
using Marten.Events.Aggregation;

namespace IGroceryStore.Baskets.Core.Projectors;

public class ProductForShopProjector : SingleStreamAggregation<ProductProjectionForShop>
{
    public static ProductProjectionForShop Create(ProductAdded productAdded)
    {
        return new ProductProjectionForShop
        {
            Id = productAdded.ProductId,
            Name = productAdded.Name,
            Category = productAdded.Category
        };
    }

    public void Apply(ProductProjectionForShop snapshot, IEvent<ProductPriceChanged> e)
    {
        //apply only if shop id is ...
        var date = DateOnly.FromDateTime(e.Timestamp.UtcDateTime);
        var price = new Price(date, e.Data.NewPrice);
        snapshot.Prices.Add(price);
    }
}

public record ProductProjectionForShop
{
    public required ulong Id { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    public List<Price> Prices { get; } = new();
}
