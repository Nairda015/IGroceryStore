using IGroceryStore.Baskets.Core.Entities;
using IGroceryStore.Baskets.Core.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace IGroceryStore.Baskets.Core.Subscribers.Products;

internal class AddProduct : IConsumer<ProductAdded>
{
    private readonly ILogger<AddProduct> _logger;
    private readonly IMongoCollection<Product> _collection;

    public AddProduct(ILogger<AddProduct> logger, IMongoCollection<Product> collection)
    {
        _logger = logger;
        _collection = collection;
    }

    public async Task Consume(ConsumeContext<ProductAdded> context)
    {
        var (productId, name, category) = context.Message;
        var streamState = await _collection.Find(x => x.Id == productId)
            .FirstOrDefaultAsync();
        if (streamState is not null) return;

        var product = new Product
        {
            Id = productId,
            Name = name,
            Category = category
        };
        
        await _collection.InsertOneAsync(product);
        _logger.LogInformation("Stream for product {ProductId} started", productId);
    }
}
