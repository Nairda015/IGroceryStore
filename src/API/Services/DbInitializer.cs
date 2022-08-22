using System.Reflection;
using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.API.Services;

internal sealed class DbInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(IServiceProvider serviceProvider, ILogger<DbInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var dbContextTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
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
    
    // private Type[] TryGetTypes(Assembly assembly)
    // {
    //     Type[] types;
    //     try
    //     {
    //         types = assembly.GetTypes();
    //     }
    //     catch (ReflectionTypeLoadException e)
    //     {
    //         _logger.LogError("Failed to load types from assembly {FullName}", assembly.FullName);
    //         types = e.Types!;
    //         _logger.LogError("Found {Length} types in ReflectionTypeLoadException with assembly {FullName}", types.Length, assembly.FullName);
    //     }
    //     return types;
    // }
}