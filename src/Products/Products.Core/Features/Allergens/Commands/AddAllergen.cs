using IGroceryStore.Products.Core.Entities;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Commands;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Products.Core.Features.Allergens.Commands;

internal record AddAllergen(AddAllergen.AddAllergenBody Body) : IHttpCommand
{
    internal record AddAllergenBody(string Name);
}

public class AddAllergenEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapPost<AddAllergen>("api/allergen").WithTags(SwaggerTags.Products);
}

internal class AddAllergenHandler : ICommandHandler<AddAllergen, IResult>
{
    private readonly ProductsDbContext _productsDbContext;
    private readonly ISnowflakeService _snowflakeService;

    public AddAllergenHandler(ProductsDbContext productsDbContext, ISnowflakeService snowflakeService)
    {
        _productsDbContext = productsDbContext;
        _snowflakeService = snowflakeService;
    }

    public async Task<IResult> HandleAsync(AddAllergen command, CancellationToken cancellationToken = default)
    {
        var allergen = new Allergen
        {
            Id = _snowflakeService.GenerateId(),
            Name = command.Body.Name
        };
        
        _productsDbContext.Allergens.Add(allergen);
        await _productsDbContext.SaveChangesAsync(cancellationToken);
        return Results.Ok(allergen.Id);
    }
}
