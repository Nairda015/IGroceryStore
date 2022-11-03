using System.Text.Json;
using EventStore.Client;
using IGroceryStore.Baskets.Events;
using IGroceryStore.Baskets.ValueObjects;
using IGroceryStore.Shops.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Baskets.Subscribers.Products;

internal class UpdateProductPrice : IConsumer<ProductPriceChanged>
{
    private readonly ILogger<UpdateProductPrice> _logger;
    private readonly EventStoreClient _client;

    public UpdateProductPrice(ILogger<UpdateProductPrice> logger, EventStoreClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<ProductPriceChanged> context)
    {
        var (productId, shopChainId, shopId, newPrice, _) = context.Message;
        
        var @event = new ProductPriceUpdated(shopId, newPrice);
        
        var eventData = new EventData(
            Uuid.NewUuid(),
            "productPriceUpdated",
            JsonSerializer.SerializeToUtf8Bytes(@event));

        var productStreamId = new ProductStreamId(productId, shopChainId);
        await _client.AppendToStreamAsync(
            productStreamId,
            StreamState.StreamExists,
            new[] { eventData });

        _logger.LogInformation("Product {productId} price updated to {price} for shop {shop}",
            productId, newPrice, shopChainId);
    }
}
