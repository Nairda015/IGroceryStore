using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IGroceryStore.Users.IntegrationTests;

public static class DbConfigExtensions
{
    public static void CleanDbContextOptions<T>(this IServiceCollection services)
        where T : DbContext
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
        if (descriptor != null) services.Remove(descriptor);
        services.RemoveAll(typeof(T));
    }

    public static void AddPostgresContext<T>(this IServiceCollection services, TestcontainerDatabase dbContainer)
        where T : DbContext
    {
        services.AddDbContext<T>(ctx =>
            ctx.UseNpgsql(dbContainer.ConnectionString));
    }
}
