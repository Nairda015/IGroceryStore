using IGroceryStore.Baskets.Core.Events;
using Marten;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Baskets.Core.Subscribers.Products;

public class UpdateProductPrice : IConsumer<Shops.Contracts.Events.ProductPriceChanged>
{
    private readonly ILogger<AddProduct> _logger;
    private readonly IDocumentSession _session;

    public UpdateProductPrice(ILogger<AddProduct> logger, IDocumentSession session)
    {
        _logger = logger;
        _session = session;
    }

    public async Task Consume(ConsumeContext<Shops.Contracts.Events.ProductPriceChanged> context)
    {
        var (productId, shopChainId, newPrice, _) = context.Message;
        
        await _session.Events.AppendOptimistic(productId.ToString(),
            new ProductPriceChanged(shopChainId, newPrice));
        await _session.SaveChangesAsync(context.CancellationToken);
        
        _logger.LogInformation("Product {productId} price updated to {price} for shop {shop}",
            productId, newPrice, shopChainId);
    }
}
