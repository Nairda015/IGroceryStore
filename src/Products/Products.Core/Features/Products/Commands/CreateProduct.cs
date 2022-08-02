using IGroceryStore.Products.Contracts.Events;
using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

public record CreateProduct(string Name,
    string Description,
    QuantityReadModel Quantity,
    BrandId BrandId,
    CountryId CountryId,
    CategoryId CategoryId) : ICommand<ulong>;

public class CreateProductEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("products/create",
            async (ICommandDispatcher dispatcher,
                CreateProduct command,
                CancellationToken cancellationToken) =>
            {
                await dispatcher.DispatchAsync(command, cancellationToken);
                return Results.Accepted();
            });
    }
}

internal class CreateProductHandler : ICommandHandler<CreateProduct, ulong>
{
    private readonly ProductsDbContext _productsDbContext;
    private readonly ISnowflakeService _snowflakeService;
    private readonly IBus _bus;

    public CreateProductHandler(ProductsDbContext productsDbContext, IBus bus, ISnowflakeService snowflakeService)
    {
        _productsDbContext = productsDbContext;
        _bus = bus;
        _snowflakeService = snowflakeService;
    }

    public async Task<ulong> HandleAsync(CreateProduct command, CancellationToken cancellationToken = default)
    {
        var (name, description, quantityReadModel, brandId, countryId, categoryId) = command;
        var categoryName = await _productsDbContext.Categories
            .Where(x => x.Id == categoryId)
            .Select(x => x.Name)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (categoryName is null) throw new CategoryNotFoundException(categoryId);
        
        var quantity = new Quantity(quantityReadModel.Amount, quantityReadModel.Unit);
        var product = new Product(_snowflakeService.GenerateId(),name, description, quantity, brandId, countryId, categoryId);

        await _productsDbContext.Products.AddAsync(product, cancellationToken);
        await _productsDbContext.SaveChangesAsync(cancellationToken);

        var productAddedEvent = new ProductAdded(product.Id, name, categoryName);
        await _bus.Publish(productAddedEvent, cancellationToken: cancellationToken);
        return product.Id;
    }
}