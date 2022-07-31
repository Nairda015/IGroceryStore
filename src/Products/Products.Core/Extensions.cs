using System.Reflection;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Commands;
using IGroceryStore.Shared.Options;
using IGroceryStore.Shared.Queries;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Products.Core;

public class ProductsModule : IModule
{
    public string Name => "Products";
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommands();
        services.AddQueries();

        services.AddScoped<ISnowflakeService, SnowflakeService>();

        var enableSensitiveData = configuration.GetValue<bool>("EnableSensitiveData");

        var options = configuration.GetOptions<PostgresOptions>("Postgres");
        services.AddDbContext<ProductsDbContext>(ctx => 
            ctx.UseNpgsql(options.ConnectionString)
                .EnableSensitiveDataLogging(enableSensitiveData));
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/{Name}", () => Name);

        var assembly = Assembly.GetAssembly(typeof(ProductsModule));
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
