using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Products.Features.Products.Queries;

internal record FindSimilar(ulong Id) : IHttpQuery;

public class FindSimilarEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapGet<FindSimilar>("find-similar/{id}");
}


internal class FindSimilarHandler : IQueryHandler<FindSimilar, IResult>
{
    private readonly ProductsDbContext _context;

    public FindSimilarHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public Task<IResult> HandleAsync(FindSimilar query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
