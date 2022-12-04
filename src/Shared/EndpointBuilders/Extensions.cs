using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Shared.EndpointBuilders;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapGet<TQuery, THandler>(this IEndpointRouteBuilder endpoints, string template) 
        where TQuery : IHttpQuery
        where THandler : IHttpQueryHandler<TQuery> =>
        endpoints.MapGet(template, async (
                THandler handler,
                [AsParameters] TQuery query,
                CancellationToken cancellationToken) =>
            await handler.HandleAsync(query, cancellationToken))
            .WithName(typeof(TQuery).FullName!);
    
    public static RouteHandlerBuilder MapPost<TCommand, THandler>(this IEndpointRouteBuilder endpoints, string template) 
        where TCommand : IHttpCommand
        where THandler : IHttpCommandHandler<TCommand> =>
        endpoints.MapPost(template, async (
                    THandler handler,
                    [AsParameters] TCommand query,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(query, cancellationToken))
            .WithName(typeof(TCommand).FullName!);
    
    public static RouteHandlerBuilder MapPut<TCommand, THandler>(this IEndpointRouteBuilder endpoints, string template) 
        where TCommand : IHttpCommand
        where THandler : IHttpCommandHandler<TCommand> =>
        endpoints.MapPut(template, async (
                    THandler handler,
                    [AsParameters] TCommand query,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(query, cancellationToken))
            .WithName(typeof(TCommand).FullName!);
    
    public static RouteHandlerBuilder MapDelete<TCommand, THandler>(this IEndpointRouteBuilder endpoints, string template) 
        where TCommand : IHttpCommand
        where THandler : IHttpCommandHandler<TCommand> =>
        endpoints.MapDelete(template, async (
                    THandler handler,
                    [AsParameters] TCommand query,
                    CancellationToken cancellationToken) =>
                await handler.HandleAsync(query, cancellationToken))
            .WithName(typeof(TCommand).FullName!);
}

// public interface IGroceryStoreEndpointBuilder
// {
//     RouteHandlerBuilder MapGet<TQuery>(string template) where TQuery : IHttpQuery;
// }
//
// public class GroceryStoreEndpointBuilder : IGroceryStoreEndpointBuilder
// {
//     private readonly IEndpointRouteBuilder _endpointRouteBuilder;
//     public GroceryStoreEndpointBuilder(IEndpointRouteBuilder endpoints)
//     {
//         _endpointRouteBuilder = endpoints;
//     }
//
//     public RouteHandlerBuilder MapGet<TQuery>(string template)
//         where TQuery : IHttpQuery =>
//         _endpointRouteBuilder.MapGet(template, async (
//                 IQueryDispatcher dispatcher,
//                 [AsParameters] TQuery query,
//                 CancellationToken cancellationToken) 
//             => await dispatcher.QueryAsync(query, cancellationToken));
// }

