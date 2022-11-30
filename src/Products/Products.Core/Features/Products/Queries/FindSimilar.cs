using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared.EndpointBuilders;
using Microsoft.AspNetCore.Http;

namespace IGroceryStore.Products.Features.Products.Queries;

internal record FindSimilar(ulong Id) : IHttpQuery;

public class FindSimilarEndpoint : IEndpoint
{
    public void RegisterEndpoint(IGroceryStoreRouteBuilder builder) =>
        builder.Products.MapGet<FindSimilar, FindSimilarHttpHandler>("find-similar/{id}");
}


internal class FindSimilarHttpHandler : IHttpQueryHandler<FindSimilar>
{
    private readonly ProductsDbContext _context;

    public FindSimilarHttpHandler(ProductsDbContext context)
    {
        _context = context;
    }

    public Task<IResult> HandleAsync(FindSimilar query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
