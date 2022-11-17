using System.Diagnostics;
using System.Reflection;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Shared.Commands;
using IGroceryStore.Shared.Configuration;
using IGroceryStore.Shared.Queries;
using IGroceryStore.Shared.Services;
using IGroceryStore.Shared.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Products;

public class ProductsModule : IModule
{
    public string Name => Source.Name;
    public static ActivitySource Source { get; } = new("Products", "1.0.0.0");

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommands();
        services.AddQueries();
        
        var options = configuration.GetOptions<PostgresSettings>();
        services.AddDbContext<ProductsDbContext>(ctx =>
            ctx.UseNpgsql(options.ConnectionString)
                .EnableSensitiveDataLogging(options.EnableSensitiveData));
        
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/api/{Name.ToLower()}/health", () => $"{Name} module is healthy")
            .WithTags(Constants.SwaggerTags.HealthChecks);

        endpoints.RegisterEndpoints<ProductsModule>();
    }
}
