using IGroceryStore.Baskets.Core.Factories;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Shared.Commands;
using IGroceryStore.Shared.Controllers;
using IGroceryStore.Shared.Options;
using IGroceryStore.Shared.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Baskets.Core;

public static class Extensions
{
    public static IServiceCollection AddBaskets(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommands();
        services.AddQueries();
        services.AddSingleton<IBasketFactory, BasketFactory>();
        
        var options = configuration.GetOptions<PostgresOptions>("Postgres");
        services.AddDbContext<BasketDbContext>(ctx => 
            ctx.UseNpgsql(options.ConnectionString));
        
        return services;
    }
}

[ApiExplorerSettings(GroupName = "Baskets")]
public abstract class BasketsControllerBase : ApiControllerBase
{
}