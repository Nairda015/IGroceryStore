using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Shops.Core;

public class ShopsModule : IModule
{
    public string Name => "Shops";
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/{Name}", () => Name);
    }
}
