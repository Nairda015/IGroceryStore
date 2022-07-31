using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ValueObjects;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Products.Core.Features.Allergens.Commands;

public record AddAllergen(string Name, string Code) : ICommand<AllergenId>;

public class AddAllergenEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("allergens/add-allergen", async(
            [FromServices] ICommandDispatcher dispatcher,
            AddAllergen command,
            CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.DispatchAsync(command, cancellationToken);
            return Results.Ok(result.Value);
        });
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
            Name = name
        };
        
        _productsDbContext.Allergens.Add(allergen);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return allergen.Id;
    }
}