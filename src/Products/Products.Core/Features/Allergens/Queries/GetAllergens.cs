using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Products.ReadModels;
using IGroceryStore.Shared.EndpointBuilders;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Features.Allergens.Queries;

internal record GetAllergens : IHttpQuery;

public class GetAllergensEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapGet<GetAllergens, GetAllergensHttpHandler>("allergens");
}

internal class GetAllergensHttpHandler : IHttpQueryHandler<GetAllergens>
{
    private readonly ProductsDbContext _context;

    public GetAllergensHttpHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> HandleAsync(GetAllergens query,
        CancellationToken cancellationToken)
    {
        var allergens =  await _context.Allergens
            .Select(x => new AllergenReadModel(x.Id, x.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new GetAllergensResult(allergens);
        return Results.Ok(result);
    }
    
    private record GetAllergensResult(IEnumerable<AllergenReadModel> Allergens);
}
