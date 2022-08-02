﻿using IGroceryStore.Products.Contracts.Events;
using IGroceryStore.Shops.Core.Entities;
using IGroceryStore.Shops.Core.Exceptions;
using IGroceryStore.Shops.Core.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace IGroceryStore.Shops.Core.Subscribers.Products;

public class AddProduct : IConsumer<ProductAdded>
{
    private readonly ILogger<AddProduct> _logger;
    private readonly IProductsRepository _productsRepository;

    public AddProduct(ILogger<AddProduct> logger, IProductsRepository productsRepository)
    {
        _logger = logger;
        _productsRepository = productsRepository;
    }

    public async Task Consume(ConsumeContext<ProductAdded> context)
    {
        var (productId, name, _) = context.Message;

        var product = new Product
        {
            Id = productId,
            Name = name,
            LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        
        var result = await _productsRepository.AddAsync(product, context.CancellationToken);
        if (!result) throw new ShopConsumerException(
            true,
            context.CorrelationId!.Value,
            "Unable to add product to database."); 
        _logger.LogInformation("Product {ProductId} added to basket database", productId);
    }
}