using System.Security.Claims;
using Bogus;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using IGroceryStore.API;
using IGroceryStore.Baskets.Core.Persistence;
using IGroceryStore.Products.Core.Persistence.Contexts;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Services;
using IGroceryStore.Users.Contracts.Events;
using IGroceryStore.Users.Core.Entities;
using IGroceryStore.Users.Core.Persistence.Contexts;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Auth;

namespace Users.IntegrationTests;

public class UserApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
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
    private readonly MockUser _user;
    public UserApiFactory()
    {
        _user = new MockUser(new Claim(Claims.Name.UserId, "1"), 
            new Claim(Claims.Name.Expire, DateTimeOffset.UtcNow.AddSeconds(2137).ToUnixTimeSeconds().ToString()));
        Randomizer.Seed = new Random(420);
        VerifierSettings.ScrubInlineGuids();
    }

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
            services.CleanDbContextOptions<UsersDbContext>();
            services.CleanDbContextOptions<BasketsDbContext>();
            services.CleanDbContextOptions<ProductsDbContext>();

            services.AddPostgresContext<UsersDbContext>(_dbContainer);
            services.AddPostgresContext<BasketsDbContext>(_dbContainer);
            services.AddPostgresContext<ProductsDbContext>(_dbContainer);

            services.AddTestAuthentication();

            services.AddSingleton<IMockUser>(_ => _user);
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
