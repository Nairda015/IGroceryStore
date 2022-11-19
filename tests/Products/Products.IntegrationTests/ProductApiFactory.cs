using Bogus;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using IGroceryStore.API;
using IGroceryStore.Products.Contracts.Events;
using IGroceryStore.Products.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MassTransit;
using Microsoft.AspNetCore.TestHost;
using IGroceryStore.Shared.Tests.Auth;
using Respawn;
using System.Data.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using System.Security.Claims;
using IGroceryStore.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Products.IntegrationTests;

public class ProductApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly MockUser _user;
    private Respawner _respawner = default!;
    private DbConnection _dbConnection = default!;
    private readonly TestcontainerDatabase _dbContainer =
        new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = Guid.NewGuid().ToString(),
                Username = "postgres",
                Password = "postgres"
            })
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

    public ProductApiFactory()
    {
        _user = new MockUser(new Claim(Claims.Name.UserId, "1"),
           new Claim(Claims.Name.Expire, DateTimeOffset.UtcNow.AddSeconds(2137).ToUnixTimeSeconds().ToString()));
        Randomizer.Seed = new Random(420);
        VerifierSettings.ScrubInlineGuids();
    }

    public HttpClient HttpClient { get; private set; } = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging => { logging.ClearProviders(); });

        builder.UseEnvironment(EnvironmentService.TestEnvironment);

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
            services.CleanDbContextOptions<ProductsDbContext>();

            //services.AddPostgresContext<UsersDbContext>(_dbContainer);
            services.AddPostgresContext<ProductsDbContext>(_dbContainer);

            services.AddTestAuthentication();

            services.AddSingleton<IMockUser>(_ => _user);
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _dbConnection = new NpgsqlConnection(_dbContainer.ConnectionString);
        HttpClient = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        await InitializeRespawner();
    }

    public async Task ResetDatabaseAsync() => await _respawner.ResetAsync(_dbConnection);

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions()
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "IGroceryStore.Products" },
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
