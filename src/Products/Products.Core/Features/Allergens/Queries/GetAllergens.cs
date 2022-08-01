using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Allergens.Queries;

public record GetAllergensResult(IEnumerable<AllergenReadModel> Allergens);
internal record GetAllergens : IQuery<GetAllergensResult>;

public class GetAllergensEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("allergens",
            async (IQueryDispatcher dispatcher, CancellationToken cancellationToken) =>
                Results.Ok(await dispatcher.QueryAsync(new GetAllergens(), cancellationToken)));
    }
}

internal class GetAllergensHandler : IQueryHandler<GetAllergens, GetAllergensResult>
{
    private readonly ProductsDbContext _context;

    public GetAllergensHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<GetAllergensResult> HandleAsync(GetAllergens query,
        CancellationToken cancellationToken = default)
    {
        var allergens =  await _context.Allergens
            .Select(x => new AllergenReadModel(x.Id, x.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new GetAllergensResult(allergens);
    }
}