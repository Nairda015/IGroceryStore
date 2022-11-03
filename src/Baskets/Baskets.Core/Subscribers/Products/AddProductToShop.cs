using System.Text.Json;
using EventStore.Client;
using IGroceryStore.Baskets.Events;
using IGroceryStore.Baskets.ValueObjects;
using IGroceryStore.Shops.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Baskets.Subscribers.Products;

internal class AddProductToShop : IConsumer<ProductAddedToShop>
{
    private readonly ILogger<AddProductToShop> _logger;
    private readonly EventStoreClient _client;

    public AddProductToShop(ILogger<AddProductToShop> logger, EventStoreClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task Consume(ConsumeContext<ProductAddedToShop> context)
    {
        var (productId, shopChainId, initialPrice) = context.Message;
        var @event = new ProductPriceHistoryStarted(shopChainId, initialPrice);
        
        var eventData = new EventData(
            Uuid.NewUuid(),
            "productPriceHistoryStarted",
            JsonSerializer.SerializeToUtf8Bytes(@event));

        var productStreamId = new ProductStreamId(productId, shopChainId);
        await _client.AppendToStreamAsync(
            productStreamId,
            StreamState.NoStream,
            new[] { eventData });
    
        _logger.LogInformation("Product {ProductId} stream for shop {ShopChainId} started", productId, shopChainId);
    }
}
