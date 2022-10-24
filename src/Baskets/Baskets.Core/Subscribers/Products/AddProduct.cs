using IGroceryStore.Baskets.Core.Events;
using Marten;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Baskets.Core.Subscribers.Products;

public class AddProduct : IConsumer<ProductAdded>
{
    private readonly ILogger<AddProduct> _logger;
    private readonly IDocumentSession _session;

    public AddProduct(ILogger<AddProduct> logger, IDocumentSession session)
    {
        _logger = logger;
        _session = session;
    }

    public async Task Consume(ConsumeContext<ProductAdded> context)
    {
        var (productId, name, category) = context.Message;

        var streamState = await _session.Events
            .FetchStreamStateAsync(productId.ToString(), context.CancellationToken);
        if (streamState is not null) return; //??

        _session.Events.StartStream(productId.ToString(), new ProductAdded(productId, name, category));
        await _session.SaveChangesAsync();
        _logger.LogInformation("Stream for product {ProductId} started", productId);
    }
}
