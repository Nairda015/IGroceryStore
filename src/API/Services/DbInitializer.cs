using System.Reflection;
using IGroceryStore.Shared.Configuration;
using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.API.Services;

internal sealed class DbInitializer
{
    private readonly IServiceProvider _serviceProvider;

    public DbInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateWithEnsuredDeletedAsync(IEnumerable<Assembly> assemblies)
    {
        using var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var contexts = GetContexts(assemblies)
            .Select(x => services.GetRequiredService(x))
            .Where(x => x is DbContext)
            .Cast<DbContext>()
            .ToList();

        await contexts.FirstOrDefault()!.Database.EnsureDeletedAsync();

        var migrations = contexts.Select(x => x.Database.EnsureCreatedAsync());
        await Task.WhenAll(migrations);
    }

    private static IEnumerable<Type> GetContexts(IEnumerable<Assembly> assemblies)
    {
        return assemblies.SelectMany(x => x.TryGetTypes())
            .Where(a => typeof(DbContext).IsAssignableFrom(a)
                        && !a.IsInterface
                        && a != typeof(DbContext));
    }
}
