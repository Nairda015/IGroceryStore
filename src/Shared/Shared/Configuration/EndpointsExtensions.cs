using System.Reflection;
using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.AspNetCore.Routing;

namespace IGroceryStore.Shared.Configuration;

public static class EndpointsExtensions
{
    public static void RegisterEndpoints<T>(this IEndpointRouteBuilder endpoints)
        where T : class, IModule
    {
        var assembly = Assembly.GetAssembly(typeof(T));
        var moduleEndpoints = assembly!
            .GetTypes()
            .Where(x => typeof(IEndpoint).IsAssignableFrom(x) && x.IsClass)
            .OrderBy(x => x.Name)
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>()
            .ToList();
        
        moduleEndpoints.ForEach(x => x.RegisterEndpoint(endpoints.ToGroceryStoreRouteBuilder()));
    }
}
