using System.Reflection;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Options;
using IGroceryStore.Shops.Core.Settings;
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
        services.RegisterOptions<DatabaseSettings>(configuration, DatabaseSettings.KeyName);
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/{Name}", () => Name);
        
        var assembly = Assembly.GetAssembly(typeof(ShopsModule));
        var moduleEndpoints = assembly!
            .GetTypes()
            .Where(x => typeof(IEndpoint).IsAssignableFrom(x) && x.IsClass)
            .OrderBy(x => x.Name)
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>()
            .ToList();
        
        moduleEndpoints.ForEach(x => x.RegisterEndpoint(endpoints));
    }
}
