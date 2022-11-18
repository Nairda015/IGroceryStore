using IGroceryStore.Shared.EndpointBuilders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Shared.Configuration;

public class GroceryStoreRouteBuilder : IGroceryStoreRouteBuilder
{
    private readonly IEndpointRouteBuilder _endpointRouteBuilder;
    
    public GroceryStoreRouteBuilder(IEndpointRouteBuilder endpointRouteBuilder)
    {
        _endpointRouteBuilder = endpointRouteBuilder;
    }
    
    public IEndpointRouteBuilder Shops => _endpointRouteBuilder
        .MapGroup("api/shops")
        .RequireAuthorization()
        .WithTags(Constants.SwaggerTags.Shops);
        
    public IEndpointRouteBuilder Products => _endpointRouteBuilder
        .MapGroup("api/products")
        .RequireAuthorization()
        .WithTags(Constants.SwaggerTags.Products);
    
    public IEndpointRouteBuilder Baskets => _endpointRouteBuilder
        .MapGroup("api/baskets")
        .RequireAuthorization()
        .WithTags(Constants.SwaggerTags.Baskets);

    public IEndpointRouteBuilder Notifications => _endpointRouteBuilder
        .MapGroup("api/notifications")
        .RequireAuthorization();

    public IEndpointRouteBuilder Users => _endpointRouteBuilder
        .MapGroup("api/users")
        .RequireAuthorization()
        .WithTags(Constants.SwaggerTags.Users);
}

// public class GroceryStoreRouteBuilder : IGroceryStoreRouteBuilder
// {
//     private readonly IEndpointRouteBuilder _endpointRouteBuilder;
//
//     public GroceryStoreRouteBuilder(IEndpointRouteBuilder endpointRouteBuilder)
//     {
//         _endpointRouteBuilder = endpointRouteBuilder;
//     }
//
//     public IGroceryStoreEndpointBuilder Shops => _endpointRouteBuilder
//         .MapGroup("api/shops")
//         .RequireAuthorization()
//         .WithTags(Constants.SwaggerTags.Shops)
//         //.WithGroupName("ShopsGroup") <- this is for versioning - add fluent builder with default value
//         .ToGroceryStoreEndpointBuilder();
//     
//     public IGroceryStoreEndpointBuilder Products => _endpointRouteBuilder
//         .MapGroup("api/products")
//         .RequireAuthorization()
//         //.RequireCors()
//         .WithTags(Constants.SwaggerTags.Products)
//         .ToGroceryStoreEndpointBuilder();
// }

public static class GroceryStoreRouteBuilderExtensions
{
    public static IGroceryStoreRouteBuilder ToGroceryStoreRouteBuilder(this IEndpointRouteBuilder endpoints) =>
        new GroceryStoreRouteBuilder(endpoints);
}

// public static class GroceryStoreEndpointBuilderExtensions
// {
//     public static IGroceryStoreEndpointBuilder ToGroceryStoreEndpointBuilder(this IEndpointRouteBuilder endpoints) =>
//         new GroceryStoreEndpointBuilder(endpoints);
// }
