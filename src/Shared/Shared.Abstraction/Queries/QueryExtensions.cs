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

public interface IGroceryStoreRouteBuilder
{
    IGroceryStoreEndpointBuilder Shops { get; }
    IGroceryStoreEndpointBuilder Products { get; }
}

public class GroceryStoreRouteBuilder : IGroceryStoreRouteBuilder
{
    private readonly IEndpointRouteBuilder _endpointRouteBuilder;

    public GroceryStoreRouteBuilder(IEndpointRouteBuilder endpointRouteBuilder)
    {
        _endpointRouteBuilder = endpointRouteBuilder;
    }

    public IGroceryStoreEndpointBuilder Shops => _endpointRouteBuilder
        .MapGroup("api/shops")
        .RequireAuthorization()
        .WithTags(Constants.SwaggerTags.Shops)
        //.WithGroupName("ShopsGroup") <- this is for versioning - add fluent builder with default value
        .ToGroceryStoreEndpointBuilder();
    
    public IGroceryStoreEndpointBuilder Products => _endpointRouteBuilder
        .MapGroup("api/products")
        .RequireAuthorization()
        //.RequireCors()
        .WithTags(Constants.SwaggerTags.Products)
        .ToGroceryStoreEndpointBuilder();
}

public static class GroceryStoreEndpointBuilderExtensions
{
    public static IGroceryStoreEndpointBuilder ToGroceryStoreEndpointBuilder(this IEndpointRouteBuilder endpoints) =>
        new GroceryStoreEndpointBuilder(endpoints);
}


public interface IGroceryStoreEndpointBuilder
{
    RouteHandlerBuilder MapGet<TQuery>(string template) where TQuery : IHttpQuery;
}

public class GroceryStoreEndpointBuilder : IGroceryStoreEndpointBuilder
{
    private readonly IEndpointRouteBuilder _endpointRouteBuilder;
    public GroceryStoreEndpointBuilder(IEndpointRouteBuilder endpoints)
    {
        _endpointRouteBuilder = endpoints;
    }

    public RouteHandlerBuilder MapGet<TQuery>(string template)
        where TQuery : IHttpQuery =>
        _endpointRouteBuilder.MapGet(template, async (
                IQueryDispatcher dispatcher,
                [AsParameters] TQuery query,
                CancellationToken cancellationToken) 
            => await dispatcher.QueryAsync(query, cancellationToken));
}

