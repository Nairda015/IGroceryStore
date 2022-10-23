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

    public async Task MigrateAsync(IEnumerable<Assembly> assemblies)
    {
        using var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var contexts = GetContexts(assemblies)
            .Select(x => services.GetRequiredService(x))
            .Where(x => x is DbContext)
            .Cast<DbContext>();
        
        // "If the database exists, then no effort is made to ensure it is compatible with the model for this context."
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.storage.relationaldatabasecreator.ensurecreatedasync?view=efcore-6.0
        // I think in production it may be a problem
        // Also, as far as I know, EnsureCreatedAsync() makes transaction in read-commited isolation level
        // which still can make race hazard so maybe it's better to make change line to 
        // contexts.ToList().ForEach(x => x.Database.EnsureCreatedAsync()); or something similar.
        var migrations = contexts.Select(x => x.Database.EnsureCreatedAsync());
        await Task.WhenAll(migrations);
    }

    public async Task MigrateWithEnsuredDeletedAsync(IEnumerable<Assembly> assemblies)
    {
        using var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var contexts = GetContexts(assemblies)
            .Select(x => services.GetRequiredService(x))
            .Where(x => x is DbContext)
            .Cast<DbContext>();

        await contexts.FirstOrDefault()!.Database.EnsureDeletedAsync();

        var migrations = contexts.Select(x => x.Database.EnsureCreatedAsync());
        await Task.WhenAll(migrations);
    }

    private IEnumerable<Type> GetContexts(IEnumerable<Assembly> assemblies)
    {
        return assemblies.SelectMany(x => x.TryGetTypes())
            .Where(a => typeof(DbContext).IsAssignableFrom(a)
                        && !a.IsInterface
                        && a != typeof(DbContext));
    }
}
