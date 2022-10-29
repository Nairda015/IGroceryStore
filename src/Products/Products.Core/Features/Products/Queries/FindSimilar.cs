using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Products.Core.Features.Products.Queries;

internal record FindSimilar(ulong Id) : IHttpQuery;

public class FindSimilarEndpoint : IEndpoint
{
    public void RegisterEndpoint(IEndpointRouteBuilder endpoints) =>
        endpoints.MapGet<FindSimilar>("api/products/find-similar/{id}").WithTags(SwaggerTags.Products);
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
