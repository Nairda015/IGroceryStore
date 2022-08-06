using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Shared.Abstraction.Commands;

public static class CommandExtensions
{
    public static RouteHandlerBuilder MapPost<TCommand>(this IEndpointRouteBuilder endpoints, string template)
        where TCommand : IHttpCommand =>
        endpoints.MapPost(template, async (
                ICommandDispatcher dispatcher,
                [AsParameters] TCommand command,
                CancellationToken cancellationToken) 
            => await dispatcher.DispatchAsync(command, cancellationToken));

    public static RouteHandlerBuilder MapPut<TCommand>(this IEndpointRouteBuilder endpoints, string template)
        where TCommand : IHttpCommand =>
        endpoints.MapPut(template, async (
                ICommandDispatcher dispatcher,
                [AsParameters] TCommand command,
                CancellationToken cancellationToken) 
            => await dispatcher.DispatchAsync(command, cancellationToken));
}