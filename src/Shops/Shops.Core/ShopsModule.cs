using System.Diagnostics;
using Amazon;
using Amazon.DynamoDBv2;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Common;
using IGroceryStore.Shared.Configuration;
using IGroceryStore.Shared.Settings;
using IGroceryStore.Shops.Repositories;
using IGroceryStore.Shops.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Shops;

public class ShopsModule : IModule
{
    public string Name => Source.Name;
    public static ActivitySource Source { get; } = new("Shops", "1.0.0.0");

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterHandlers<ShopsModule>();

        services.RegisterOptions<DynamoDbSettings>(configuration);
        var dynamoDbSettings = configuration.GetOptions<DynamoDbSettings>();
        
        if (dynamoDbSettings.LocalMode)
        {
            services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = dynamoDbSettings.LocalServiceUrl
            }));
        }
        else
        {
            services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(RegionEndpoint.EUCentral1));
        }

        services.AddSingleton<IUsersRepository, UsersRepository>();
        services.AddSingleton<IProductsRepository, ProductsRepository>();
        services.AddSingleton<IShopsRepository, ShopsRepository>();
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/api/health/{Name.ToLower()}", () => $"{Name} module is healthy")
            .WithTags(Constants.SwaggerTags.HealthChecks);

        endpoints.RegisterEndpoints<ShopsModule>();
    }
}
