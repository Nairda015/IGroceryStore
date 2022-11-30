using System.Diagnostics;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Common;
using IGroceryStore.Shared.Configuration;
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
        services.RegisterHandlers<ProductsModule>();
        
        var options = configuration.GetOptions<PostgresSettings>();
        services.AddDbContext<ProductsDbContext>(ctx =>
            ctx.UseNpgsql(options.ConnectionString)
                .EnableSensitiveDataLogging(options.EnableSensitiveData));
    }

    public void Use(IApplicationBuilder app)
    {
        //Maybe use this only in dev?
        app.UseMigrationsEndPoint();
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/api/health/{Name.ToLower()}", () => $"{Name} module is healthy")
            .WithTags(Constants.SwaggerTags.HealthChecks);

        endpoints.RegisterEndpoints<ProductsModule>();
    }
}
