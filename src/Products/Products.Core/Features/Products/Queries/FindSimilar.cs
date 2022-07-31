using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Products.Core.Features.Products.Queries;

public record FindSimilar(ulong Id) : IQuery<ulong>;

public class FindSimilarEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("products/find-similar/{id}",
            async (IQueryDispatcher dispatcher, ulong id, CancellationToken cancellationToken) =>
                Results.Ok(await dispatcher.QueryAsync(new FindSimilar(id), cancellationToken)));
    }
}


internal class FindSimilarHandler : IQueryHandler<FindSimilar, ulong>
{
    private readonly ProductsDbContext _context;

    public FindSimilarHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public Task<ulong> HandleAsync(FindSimilar query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}