using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Mvc;

namespace IGroceryStore.Products.Core.Features.Products.Queries;

public record FindSimilar(Guid Id) : IQuery<Guid>;

public class FindSimilarController : ProductsControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public FindSimilarController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("products/find-similar/{id:guid}")]
    public async Task<ActionResult<Guid>> Get([FromRoute] Guid id)
    {
        var result = await _queryDispatcher.QueryAsync(new FindSimilar(id));
        return Ok(result);
    }
}


internal class FindSimilarHandler : IQueryHandler<FindSimilar, Guid>
{
    private readonly ProductsDbContext _context;

    public FindSimilarHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public Task<Guid> HandleAsync(FindSimilar query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}