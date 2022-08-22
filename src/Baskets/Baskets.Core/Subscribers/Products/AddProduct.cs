using IGroceryStore.Baskets.Core.Entities;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Products.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Baskets.Core.Subscribers.Products;

public class AddProduct : IConsumer<ProductAdded>
{
    private readonly ILogger<AddProduct> _logger;
    private readonly BasketsDbContext _basketsDbContext;

    public AddProduct(ILogger<AddProduct> logger, BasketsDbContext basketsDbContext)
    {
        _logger = logger;
        _basketsDbContext = basketsDbContext;
    }

    public async Task Consume(ConsumeContext<ProductAdded> context)
    {
        var (productId, name, category) = context.Message;
        if (await _basketsDbContext.Products.AnyAsync(x => x.Id.Equals(productId))) return;

        var product = new Product(productId, name, category);
        await _basketsDbContext.Products.AddAsync(product);
        await _basketsDbContext.SaveChangesAsync();
        _logger.LogInformation("Product {ProductId} added to basket database", productId);
    }
}