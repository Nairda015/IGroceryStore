using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IGroceryStore.Shared.Services;

internal sealed class AppInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public AppInitializer(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var dbContextTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(a => typeof(DbContext).IsAssignableFrom(a) &&
                        !a.IsInterface &&
                        a != typeof(DbContext) &&
                        a.Namespace != "Microsoft.AspNetCore.Identity.EntityFrameworkCore");

        using var scope = _serviceProvider.CreateScope();
        foreach (var dbContextType in dbContextTypes)
        {
            if (scope.ServiceProvider.GetRequiredService(dbContextType) is not DbContext dbContext ||
                !dbContext.Database.IsRelational())
            {
                continue;
            }

            if (dbContext is IGroceryStoreDbContext groceryStoreDbContext)
            {
                await groceryStoreDbContext.Seed();
            } 

            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}