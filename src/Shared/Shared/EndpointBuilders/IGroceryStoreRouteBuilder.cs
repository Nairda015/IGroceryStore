using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Shared.EndpointBuilders;

public interface IGroceryStoreRouteBuilder
{
    public IEndpointRouteBuilder Shops { get; }
    public IEndpointRouteBuilder Products { get; }
    public IEndpointRouteBuilder Baskets { get; }
    public IEndpointRouteBuilder Notifications { get; }
    public IEndpointRouteBuilder Users { get; }
}

// public interface IGroceryStoreRouteBuilder
// {
//     IGroceryStoreEndpointBuilder Shops { get; }
//     IGroceryStoreEndpointBuilder Products { get; }
// }
