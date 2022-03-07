using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Core.ReadModels;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Shared.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Features.Allergens.Queries;

internal record GetAllergens : IQuery<IEnumerable<AllergenReadModelWithId>>;

public class GetAllergensController : ApiControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public GetAllergensController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("allergens")]
    public async Task<ActionResult<IEnumerable<AllergenReadModel>>> GetAllergens(CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.QueryAsync(new GetAllergens(), cancellationToken);
        return Ok(result);
    }
}

internal class GetAllergensHandler : IQueryHandler<GetAllergens, IEnumerable<AllergenReadModelWithId>>
{
    private readonly ProductsDbContext _context;

    public GetAllergensHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AllergenReadModelWithId>> HandleAsync(GetAllergens query,
        CancellationToken cancellationToken = default)
    {
        return await _context.Allergens
            .Select(x => new AllergenReadModelWithId(x.Id, x.Name, x.Code))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}