using Bogus;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using IGroceryStore.API;
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
            }).Build();

    public UserApiFactory()
    {
        Randomizer.Seed = new Random(1);
        VerifierSettings.ScrubInlineGuids();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging => { logging.ClearProviders(); });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(UsersDbContext));
            services.AddDbContext<UsersDbContext>(ctx =>
            {
                ctx.UseNpgsql(_dbContainer.ConnectionString, x =>
                    x.EnableRetryOnFailure(10, TimeSpan.FromSeconds(10), null));
                ctx.EnableDetailedErrors();
                ctx.EnableSensitiveDataLogging();
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();
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

