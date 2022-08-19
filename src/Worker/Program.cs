using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Configuration;
using IGroceryStore.Shared.Services;
using IGroceryStore.Shared.Settings;
using IGroceryStore.Worker;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureModules();

//AWS
// if (!builder.Environment.IsDevelopment())
// {
//     builder.Configuration.AddSystemsManager("/Production/IGroceryStore", TimeSpan.FromSeconds(30));
// }

var (assemblies, moduleAssemblies, modules) = AppInitializer.Initialize(builder);

foreach (var module in modules)
{
    module.Register(builder.Services, builder.Configuration);
}

//Services
builder.Services.AddSingleton<DateTimeService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient(s => s.GetService<HttpContext>()!.User);
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

 
//Messaging
var rabbitSettings = builder.Configuration.GetOptions<RabbitSettings>();
builder.Services.AddMassTransit(bus =>
{
    bus.SetKebabCaseEndpointNameFormatter();
    
    //TODO: remove later
    bus.SetInMemorySagaRepositoryProvider();

    foreach (var assembly in moduleAssemblies)
    {
        bus.AddConsumers(assembly);
        bus.AddSagaStateMachines(assembly);
        bus.AddSagas(assembly);
        bus.AddActivities(assembly);
    }

    bus.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitSettings.Host, rabbitSettings.VirtualHost, h =>
        {
            h.Username(rabbitSettings.Username);
            h.Password(rabbitSettings.Password);
        });
        
        cfg.ConfigureEndpoints(ctx);
    });
});

//Logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole()
        .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
    loggingBuilder.AddDebug();
});

builder.Services.AddOpenTelemetryTracing(x =>
{
    x.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("Worker")
            .AddTelemetrySdk()
            .AddEnvironmentVariableDetector())
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddSource("MassTransit")
        .AddEntityFrameworkCoreInstrumentation()
        .AddNpgsql()
        .AddJaeger();
});
 
var app = builder.Build();
System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Run("http://localhost:5010");