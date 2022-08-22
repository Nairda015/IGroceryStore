using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Configuration;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.API.Services;

internal sealed class DbInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public DbInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var dbContextTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.TryGetTypes())
            .Where(a => typeof(DbContext).IsAssignableFrom(a) &&
                        !a.IsInterface &&
                        a != typeof(DbContext) &&
                        a.Namespace != "Microsoft.AspNetCore.Identity.EntityFrameworkCore");

        using var scope = _serviceProvider.CreateScope();
        foreach (var dbContextType in dbContextTypes)
        {
            if (scope.ServiceProvider.GetRequiredService(dbContextType) is not DbContext dbContext
                || !dbContext.Database.IsRelational()) continue;

            await dbContext.Database.MigrateAsync(cancellationToken);
            if (dbContext is IGroceryStoreDbContext groceryStoreDbContext)
            {
                await groceryStoreDbContext.Seed();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}