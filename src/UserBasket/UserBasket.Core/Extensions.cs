﻿using IGroceryStore.Shared.Commands;
using IGroceryStore.Shared.Options;
using IGroceryStore.Shared.Queries;
using IGroceryStore.UserBasket.Core.Factories;
using IGroceryStore.UserBasket.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.UserBasket.Core;

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