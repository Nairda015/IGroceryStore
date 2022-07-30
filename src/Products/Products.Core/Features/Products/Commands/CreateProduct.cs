using DotNetCore.CAP;
using IGroceryStore.Products.Contracts.Events;
using IGroceryStore.Products.Contracts.ReadModels;
using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Exceptions;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

public record CreateProduct(string Name,
    string Description,
    QuantityReadModel Quantity,
    BrandId BrandId,
    CountryId CountryId,
    CategoryId CategoryId) : ICommand<ulong>;

public class CreateProductController : ProductsControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public CreateProductController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost("products/create")]
    public async Task<ActionResult<ulong>> CreateProduct([FromBody] CreateProduct command,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.DispatchAsync(command, cancellationToken);
        return Ok(result);
    }
}

internal class CreateProductHandler : ICommandHandler<CreateProduct, ulong>
{
    private readonly ProductsDbContext _productsDbContext;
    private readonly ISnowflakeService _snowflakeService;
    private readonly ICapPublisher _publisher;

    public CreateProductHandler(ProductsDbContext productsDbContext, ICapPublisher publisher, ISnowflakeService snowflakeService)
    {
        _productsDbContext = productsDbContext;
        _publisher = publisher;
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
        await _publisher.PublishAsync(nameof(ProductAdded), productAddedEvent, cancellationToken: cancellationToken);
        return product.Id;
    }
}