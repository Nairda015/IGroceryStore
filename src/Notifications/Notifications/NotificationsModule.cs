using System.Diagnostics;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Common;
using IGroceryStore.Shared.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Notifications;

public class NotificationsModule : IModule
{
    public string Name => Source.Name;
    public static ActivitySource Source { get; } = new("Notifications", "1.0.0.0");

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterHandlers<NotificationsModule>();
    }

    public void Use(IApplicationBuilder app)
    {
    }

    public void Expose(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"/api/health/{Name.ToLower()}", () => $"{Name} module is healthy")
            .WithTags(Constants.SwaggerTags.HealthChecks);
        
        endpoints.RegisterEndpoints<NotificationsModule>();
    }
}
