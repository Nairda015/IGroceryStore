using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Allergens.Queries;

internal record GetAllergens : IHttpQuery;

public class GetAllergensEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<GetAllergens>("api/allergens").WithTags(SwaggerTags.Products);
}

internal class GetAllergensHandler : IQueryHandler<GetAllergens, IResult>
{
    private readonly ProductsDbContext _context;

    public GetAllergensHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<IResult> HandleAsync(GetAllergens query,
        CancellationToken cancellationToken = default)
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
