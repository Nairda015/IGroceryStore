using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace IGroceryStore.Products.Core.Features.Allergens.Commands;

public record AddAllergen(string Name, string Code) : ICommand<AllergenId>;

public class AddAllergenController : ProductsControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public AddAllergenController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost("allergens/add-allergen")]
    public async Task<ActionResult> AddAllergen([FromBody] AddAllergen command, CancellationToken cancellationToken)
    {
        var result = await _commandDispatcher.DispatchAsync(command, cancellationToken);
        return Ok(result.Value);
    }
}

internal class AddAllergenHandler : ICommandHandler<AddAllergen, AllergenId>
{
    private readonly ProductsDbContext _productsDbContext;
    private readonly ISnowflakeService _snowflakeService;

    public AddAllergenHandler(ProductsDbContext productsDbContext, ISnowflakeService snowflakeService)
    {
        _productsDbContext = productsDbContext;
        _snowflakeService = snowflakeService;
    }

    public async Task<AllergenId> HandleAsync(AddAllergen command, CancellationToken cancellationToken = default)
    {
        var (name, code) = command;
        var allergen = new Allergen()
        {
            Id = _snowflakeService.GenerateId(),
            Name = name,
            Code = code
        };
        
        _productsDbContext.Allergens.Add(allergen);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return allergen.Id;
    }
}