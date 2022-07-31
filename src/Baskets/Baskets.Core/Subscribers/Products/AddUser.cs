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
    private readonly BasketDbContext _basketDbContext;

    public AddProduct(ILogger<AddProduct> logger, BasketDbContext basketDbContext)
    {
        _logger = logger;
        _basketDbContext = basketDbContext;
    }

    public async Task Consume(ConsumeContext<ProductAdded> context)
    {
        var (productId, name, category) = context.Message;
        if (await _basketDbContext.Products.AnyAsync(x => x.Id.Equals(productId))) return;

        var product = new Product(productId, name, category);
        await _basketDbContext.Products.AddAsync(product);
        await _basketDbContext.SaveChangesAsync();
        _logger.LogInformation("Product {ProductId} added to basket database", productId);
    }
}