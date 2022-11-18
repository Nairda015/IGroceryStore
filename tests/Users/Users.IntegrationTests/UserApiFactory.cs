using System.Data.Common;
using System.Security.Claims;
using Bogus;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using IGroceryStore.API;
using IGroceryStore.Products.Persistence.Contexts;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Services;
using IGroceryStore.Shared.Tests.Auth;
using IGroceryStore.Users.Contracts.Events;
using IGroceryStore.Users.Persistence.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;

namespace IGroceryStore.Users.IntegrationTests;

public class UserApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
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

    public UserApiFactory()
    {
        _user = new MockUser(new Claim(Constants.Claims.Name.UserId, "1"), 
            new Claim(Constants.Claims.Name.Expire, DateTimeOffset.UtcNow.AddSeconds(2137).ToUnixTimeSeconds().ToString()));
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
                x.AddHandler<UserCreated>(context => context.ConsumeCompleted);
            });

            
        });

        builder.ConfigureTestServices(services =>
        {
            // we need all because we are running migrations in app startup
            services.CleanDbContextOptions<UsersDbContext>();
            services.CleanDbContextOptions<ProductsDbContext>();

            services.AddPostgresContext<UsersDbContext>(_dbContainer);
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
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "IGroceryStore.Users" },
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
