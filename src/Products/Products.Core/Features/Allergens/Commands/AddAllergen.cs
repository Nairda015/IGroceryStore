using IGroceryStore.Products.Entities;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared.EndpointBuilders;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Http;

namespace IGroceryStore.Products.Features.Allergens.Commands;

internal record AddAllergen(AddAllergen.AddAllergenBody Body) : IHttpCommand
{
    internal record AddAllergenBody(string Name);
}

public class AddAllergenEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapPost<AddAllergen, AddAllergenHandler>("allergens");
}

internal class AddAllergenHandler : IHttpCommandHandler<AddAllergen>
{
    private readonly ProductsDbContext _productsDbContext;
    private readonly ISnowflakeService _snowflakeService;

    public AddAllergenHandler(ProductsDbContext productsDbContext, ISnowflakeService snowflakeService)
    {
        _productsDbContext = productsDbContext;
        _snowflakeService = snowflakeService;
    }

    public async Task<IResult> HandleAsync(AddAllergen command, CancellationToken cancellationToken)
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
