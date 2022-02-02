using IGroceryStore.Shared.Commands;
using IGroceryStore.Shared.Options;
using IGroceryStore.Shared.Queries;
using IGroceryStore.Users.Core.Factories;
using IGroceryStore.Users.Core.Persistence;
using IGroceryStore.Users.Core.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Users.Core;

public static class Extensions
{
    public static IServiceCollection AddUsers(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommands();
        services.AddQueries();
        services.AddSingleton<IUserFactory, UserFactory>();

        var enableSensitiveData = configuration.GetValue<bool>("EnableSensitiveData");

        var options = configuration.GetOptions<PostgresOptions>("Postgres");
        services.AddDbContext<UsersDbContext>(ctx => 
            ctx.UseNpgsql(options.ConnectionString)
            .EnableSensitiveDataLogging(enableSensitiveData));

        return services;
    }
}