using System.Diagnostics;
using System.Reflection;
using IGroceryStore.Baskets.Entities;
using IGroceryStore.Baskets.Projectors;
using IGroceryStore.Baskets.Settings;
using IGroceryStore.Shared.Abstraction;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Queries;
using IGroceryStore.Shared.Commands;
using IGroceryStore.Shared.Configuration;
using IGroceryStore.Shared.Queries;
using IGroceryStore.Shared.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace IGroceryStore.Baskets;

public class BasketsModule : IModule
{
    public string Name => Source.Name;
    public static ActivitySource Source { get; } = new("Baskets", "1.0.0.0");

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommands();
        services.AddQueries();

        RegisterMongoCollections(services, configuration);

        var eventStoreSettings = configuration.GetOptions<EventStoreSettings>();
        services.AddEventStoreClient(eventStoreSettings.ConnectionString, x => 
        {
            x.DefaultDeadline = TimeSpan.FromSeconds(5);
        });
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/api/{Name.ToLower()}/health", () => $"{Name} module is healthy")
            .WithTags(Constants.SwaggerTags.HealthChecks);

        endpoints.RegisterEndpoints<BasketsModule>();
    }

    private static void RegisterMongoCollections(IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetOptions<MongoDbSettings>();
        var mongoClient = new MongoClient(settings.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
        
        services.AddScoped<IMongoCollection<User>>(_ => mongoDatabase.GetCollection<User>(settings.UsersCollectionName));
        services.AddScoped<IMongoCollection<Basket>>(_ => mongoDatabase.GetCollection<Basket>(settings.BasketsCollectionName));
        services.AddScoped<IMongoCollection<Product>>(_ => mongoDatabase.GetCollection<Product>(settings.ProductsCollectionName));
        services.AddScoped<IMongoCollection<ProductProjectionForShop>>(_ => mongoDatabase.GetCollection<ProductProjectionForShop>(settings.ProjectionsCollectionName));
    }
}
