using System.Diagnostics;
using System.Reflection;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Commands;
using IGroceryStore.Shared.Queries;
using IGroceryStore.Shared.Settings;
using IGroceryStore.Users.Factories;
using IGroceryStore.Users.JWT;
using IGroceryStore.Users.Persistence.Contexts;
using IGroceryStore.Users.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        services.AddCommands();
        services.AddQueries();
        services.AddSingleton<IUserFactory, UserFactory>();
        services.AddScoped<ITokenManager, JwtTokenManager>();

        services.AddAuthorization();

        var options = configuration.GetOptions<PostgresSettings>();
        services.AddDbContext<UsersDbContext>(ctx =>
            ctx.UseNpgsql(options.ConnectionString)
                .EnableSensitiveDataLogging(options.EnableSensitiveData));


        var jwtSettings = configuration.GetOptions<JwtSettings>();
        var authenticationBuilder = services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions =>
                JwtSettings.Configure(jwtBearerOptions, Tokens.Audience.Access, jwtSettings))
            .AddJwtBearer(Tokens.Audience.Refresh,
                jwtBearerOptions => JwtSettings.Configure(jwtBearerOptions, Tokens.Audience.Refresh, jwtSettings));
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/api/{Name.ToLower()}/health", () => $"{Name} module is healthy")
            .WithTags(SwaggerTags.HealthChecks);

        var assembly = Assembly.GetAssembly(typeof(UsersModule));
        var moduleEndpoints = assembly!
            .GetTypes()
            .Where(x => typeof(IEndpoint).IsAssignableFrom(x) && x.IsClass)
            .OrderBy(x => x.Name)
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>()
            .ToList();

        moduleEndpoints.ForEach(x => x.RegisterEndpoint(endpoints));
    }
}
