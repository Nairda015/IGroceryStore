using System.Reflection;
using IGroceryStore.Shared.Abstraction.Common;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Commands;
using IGroceryStore.Shared.Controllers;
using IGroceryStore.Shared.Options;
using IGroceryStore.Shared.Queries;
using IGroceryStore.Users.Core.Factories;
using IGroceryStore.Users.Core.Persistence.Contexts;
using IGroceryStore.Users.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace IGroceryStore.Users.Core;

public class UsersModule : IModule
{
    public string Name => "Users";
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommands();
        services.AddQueries();
        services.AddSingleton<IUserFactory, UserFactory>();
        services.AddScoped<ITokenManager, JwtTokenManager>();

        services.AddAuthorization();

        var enableSensitiveData = configuration.GetValue<bool>("EnableSensitiveData");

        var options = configuration.GetOptions<PostgresOptions>("Postgres");
        services.AddDbContext<UsersDbContext>(ctx => 
            ctx.UseNpgsql(options.ConnectionString)
                .EnableSensitiveDataLogging(enableSensitiveData));
        
        
        var jwtSettings = configuration.GetOptions<JwtSettings>("Users:JwtSettings");
        var authenticationBuilder = services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions => JwtSettings.Configure(jwtBearerOptions, Tokens.Audience.Access, jwtSettings))
            .AddJwtBearer(Tokens.Audience.Refresh, jwtBearerOptions => JwtSettings.Configure(jwtBearerOptions, Tokens.Audience.Refresh, jwtSettings));
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/{Name}", () => Name);
        
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

[ApiExplorerSettings(GroupName = "Users")]
public abstract class UsersControllerBase : ApiControllerBase
{
}