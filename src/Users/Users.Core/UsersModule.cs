using System.Diagnostics;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Common;
using IGroceryStore.Shared.Configuration;
using IGroceryStore.Shared.Settings;
using IGroceryStore.Users.Persistence.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Users;

public class UsersModule : IModule
{
    public string Name => Source.Name;
    public static ActivitySource Source { get; } = new("Users", "1.0.0.0");

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterHandlers<UsersModule>();

        var options = configuration.GetOptions<PostgresSettings>();
        services.AddDbContext<UsersDbContext>(ctx =>
            ctx.UseNpgsql(options.ConnectionString)
                .EnableSensitiveDataLogging(options.EnableSensitiveData));
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/api/health/{Name.ToLower()}", () => $"{Name} module is healthy")
            .WithTags(Constants.SwaggerTags.HealthChecks);

        endpoints.RegisterEndpoints<UsersModule>();
    }
}
