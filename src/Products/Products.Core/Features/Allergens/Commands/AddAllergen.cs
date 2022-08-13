using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Products.Core.Features.Allergens.Commands;

internal record AddAllergen(string Name);
internal record AddAllergenCommand(AddAllergen Value) : IHttpCommand;

public class AddAllergenEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<AddAllergenCommand>("allergen").WithTags(SwaggerTags.Products);
}

internal class AddAllergenHandler : ICommandHandler<AddAllergenCommand, IResult>
{
    private readonly ProductsDbContext _productsDbContext;
    private readonly ISnowflakeService _snowflakeService;

    public AddAllergenHandler(ProductsDbContext productsDbContext, ISnowflakeService snowflakeService)
    {
        _productsDbContext = productsDbContext;
        _snowflakeService = snowflakeService;
    }

    public async Task<IResult> HandleAsync(AddAllergenCommand command, CancellationToken cancellationToken = default)
    {
        var allergen = new Allergen
        {
            Id = _snowflakeService.GenerateId(),
            Name = command.Value.Name
        };
        
        _productsDbContext.Allergens.Add(allergen);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(allergen.Id);
    }
}