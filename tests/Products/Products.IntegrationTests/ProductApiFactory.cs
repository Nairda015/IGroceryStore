using Bogus;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using IGroceryStore.API;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Products.Contracts.Events;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;
using MassTransit;
using Microsoft.AspNetCore.TestHost;

namespace Products.IntegrationTests;

public class ProductApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
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

    public ProductApiFactory()
    {
        Randomizer.Seed = new Random(420);
        VerifierSettings.ScrubInlineGuids();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging => { logging.ClearProviders(); });

        builder.ConfigureServices(services =>
        {
            services.AddMassTransitTestHarness(x =>
            {
                x.AddHandler<ProductAdded>(context => context.ConsumeCompleted);
            });
        });

        builder.ConfigureTestServices(services =>
        {
            //services.CleanDbContextOptions<UsersDbContext>();
            services.CleanDbContextOptions<BasketsDbContext>();
            services.CleanDbContextOptions<ProductsDbContext>();

            //services.AddPostgresContext<UsersDbContext>(_dbContainer);
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
