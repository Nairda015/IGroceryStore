using System.Diagnostics;
using System.Reflection;
using IGroceryStore.Baskets.Core.Factories;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Baskets.Core.Subscribers.Users;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Commands;
using IGroceryStore.Shared.Queries;
using IGroceryStore.Shared.Settings;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Baskets.Core;

public class BasketsModule : IModule
{
    public string Name => Source.Name;
    public static ActivitySource Source { get; } = new("Baskets", "1.0.0.0");

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommands();
        services.AddQueries();
        services.AddSingleton<IBasketFactory, BasketFactory>();

        var options = configuration.GetOptions<PostgresSettings>();
        services.AddDbContext<BasketsDbContext>(ctx =>
            ctx.UseNpgsql(options.ConnectionString));

        //Subscriptions
        services.AddTransient<AddUser>();
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/api/{Name.ToLower()}/health", () => $"{Name} module is healthy")
            .WithTags(SwaggerTags.HealthChecks);

        var assembly = Assembly.GetAssembly(typeof(BasketsModule));
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