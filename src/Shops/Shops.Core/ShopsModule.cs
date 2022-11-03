using System.Diagnostics;
using System.Reflection;
using Amazon;
using Amazon.DynamoDBv2;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
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
        services.RegisterOptions<DynamoDbSettings>(configuration);
        var dynamoDbSettings = configuration.GetOptions<DynamoDbSettings>();
        if (dynamoDbSettings.LocalMode)
        {
            services.AddSingleton<IAmazonDynamoDB>(sp =>
            {
                var clientConfig = new AmazonDynamoDBConfig { ServiceURL = dynamoDbSettings.LocalServiceUrl };
                return new AmazonDynamoDBClient(clientConfig);
            });
        }
        else
        {
            services.AddSingleton<IAmazonDynamoDB>(_ => new AmazonDynamoDBClient(RegionEndpoint.EUCentral1));
        }

        services.AddSingleton<IUsersRepository, UsersRepository>();
        services.AddSingleton<IProductsRepository, ProductsRepository>();
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/api/{Name.ToLower()}/health", () => $"{Name} module is healthy")
            .WithTags(SwaggerTags.HealthChecks);

        var assembly = Assembly.GetAssembly(typeof(ShopsModule));
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
