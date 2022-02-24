using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Commands;
using IGroceryStore.Shared.Options;
using IGroceryStore.Shared.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Products.Core;

public static class Extensions
{
    public static IServiceCollection AddProducts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommands();
        services.AddQueries();
        
        

        var enableSensitiveData = configuration.GetValue<bool>("EnableSensitiveData");

        var options = configuration.GetOptions<PostgresOptions>("Postgres");
        services.AddDbContext<ProductsDbContext>(ctx => 
            ctx.UseNpgsql(options.ConnectionString)
            .EnableSensitiveDataLogging(enableSensitiveData));

        return services;
    }
}