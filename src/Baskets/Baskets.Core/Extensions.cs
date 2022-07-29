using IGroceryStore.Baskets.Core.Factories;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Baskets.Core.Subscribers.Users;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Commands;
using IGroceryStore.Shared.Controllers;
using IGroceryStore.Shared.Options;
using IGroceryStore.Shared.Queries;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Baskets.Core;

public class BasketsModule : IModule
{
    public string Name => "Baskets";
    
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommands();
        services.AddQueries();
        services.AddSingleton<IBasketFactory, BasketFactory>();
        
        var options = configuration.GetOptions<PostgresOptions>("Postgres");
        services.AddDbContext<BasketDbContext>(ctx => 
            ctx.UseNpgsql(options.ConnectionString));
        
        //Subscriptions
        services.AddTransient<AddUser>();
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/{Name}", () => Name);
    }
}

[ApiExplorerSettings(GroupName = "Baskets")]
public abstract class BasketsControllerBase : ApiControllerBase
{
}