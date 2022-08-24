using Bogus;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using IGroceryStore.API;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.Persistence.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Users.IntegrationTests;

public class UserApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly TestcontainerDatabase _dbContainer =
        new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "db",
                Username = "postgres",
                Password = "postgres"
            })
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

    // private readonly TestcontainerMessageBroker _rabbit =
    //     new TestcontainersBuilder<RabbitMqTestcontainer>()
    //         .ConfigureContainer(x =>
    //         {
    //             x.Username = "rabbitmq";
    //             x.Password = "rabbitmq";
    //         })
    //         .Build();

    public UserApiFactory()
    {
        Randomizer.Seed = new Random(1);
        VerifierSettings.ScrubInlineGuids();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureTestServices(services =>
        {
            services.CleanDbContextOptions<UsersDbContext>();
            services.CleanDbContextOptions<BasketsDbContext>();
            services.CleanDbContextOptions<ProductsDbContext>();

            services.AddPostgresContext<UsersDbContext>(_dbContainer);
            services.AddPostgresContext<BasketsDbContext>(_dbContainer);
            services.AddPostgresContext<ProductsDbContext>(_dbContainer);
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}

[CollectionDefinition("UserCollection")]
public class UserCollection : ICollectionFixture<UserApiFactory>
{
}

public static class DbCleaner
{
    public static void CleanDbContextOptions<T>(this IServiceCollection services)
    where T : DbContext
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
        if (descriptor != null) services.Remove(descriptor);
        services.RemoveAll(typeof(T));
    }
    
    public static void AddPostgresContext<T>(this IServiceCollection services, TestcontainerDatabase dbContainer)
        where T : DbContext
    {
        services.AddDbContext<T>(ctx =>
            ctx.UseNpgsql(dbContainer.ConnectionString, 
                x =>
                {
                    
                    x.CommandTimeout(30);
                }));
    }
}