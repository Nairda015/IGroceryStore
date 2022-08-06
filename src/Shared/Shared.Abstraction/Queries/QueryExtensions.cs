using IGroceryStore.Shared.Abstraction.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Shared.Abstraction.Queries;

public static class QueryExtensions
{
    public static RouteHandlerBuilder MapGet<TQuery>(this IEndpointRouteBuilder endpoints, string template)
        where TQuery : IHttpQuery =>
        endpoints.MapGet(template, async (
                IQueryDispatcher dispatcher,
                [AsParameters] TQuery query,
                CancellationToken cancellationToken) 
            => await dispatcher.QueryAsync(query, cancellationToken));
}