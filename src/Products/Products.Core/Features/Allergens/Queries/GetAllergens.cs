using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Allergens.Queries;

public record GetAllergensResult(IEnumerable<AllergenReadModelWithId> Allergens);
internal record GetAllergens : IQuery<GetAllergensResult>;

public class GetAllergensController : ProductsControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public GetAllergensController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("allergens")]
    public async Task<ActionResult<GetAllergensResult>> GetAllergens(CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.QueryAsync(new GetAllergens(), cancellationToken);
        return Ok(result);
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
            .Select(x => new AllergenReadModelWithId(x.Id, x.Name, x.Code))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new GetAllergensResult(allergens);
    }
}