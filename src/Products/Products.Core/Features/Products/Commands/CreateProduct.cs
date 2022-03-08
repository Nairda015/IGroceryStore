using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Commands;
using Microsoft.AspNetCore.Mvc;

namespace IGroceryStore.Products.Core.Features.Products.Commands;

public record CreateProduct(string Name,
    string Description,
    QuantityReadModel Quantity,
    BrandId BrandId,
    CountryId CountryId,
    CategoryId CategoryId) : ICommand<Guid>;

public class CreateProductController : ProductsControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public CreateProductController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost("products/create")]
    public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProduct command,
        CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.DispatchAsync(command, cancellationToken);
        return Ok(result);
    }
}

internal class CreateProductHandler : ICommandHandler<CreateProduct, Guid>
{
    private readonly ProductsDbContext _productsDbContext;

    public CreateProductHandler(ProductsDbContext productsDbContext)
    {
        _productsDbContext = productsDbContext;
    }

    public async Task<Guid> HandleAsync(CreateProduct command, CancellationToken cancellationToken = default)
    {
        var (name, description, quantityReadModel, brandId, countryId, categoryId) = command;
        var quantity = new Quantity(quantityReadModel.Amount, quantityReadModel.Unit);
        var product = new Product(name, description, quantity, brandId, countryId, categoryId);

        await _productsDbContext.Products.AddAsync(product, cancellationToken);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}