using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Shared.Abstraction.Common;

public interface IEndpoint
{
    void RegisterEndpoint(IEndpointRouteBuilder endpoints);
}