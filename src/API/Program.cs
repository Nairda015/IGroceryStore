using FluentValidation;
using IGroceryStore.API;
using IGroceryStore.API.Middlewares;
using IGroceryStore.API.Services;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Services;
using IGroceryStore.Shared.Configuration;
using IGroceryStore.Shared.Settings;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureModules();

var (assemblies, moduleAssemblies, modules) = AppInitializer.Initialize(builder);

foreach (var module in modules)
{
    module.Register(builder.Services, builder.Configuration);
}

//AWS
if (!builder.Environment.IsDevelopment() && !builder.Environment.IsTestEnvironment())
{
    
    builder.Configuration.AddSystemsManager("/Production/IGroceryStore", TimeSpan.FromSeconds(30));
}

//DateTime
builder.Services.AddSingleton<DateTimeService>();

//Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.OrderActionsBy(x => x.HttpMethod); });

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient(s => s.GetService<HttpContext>()!.User);
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<DbInitializer>();

//Middlewares
builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services.AddValidatorsFromAssemblies(moduleAssemblies, includeInternalTypes: true);

//Messaging
var rabbitSettings = builder.Configuration.GetOptions<RabbitSettings>();
builder.Services.AddMassTransit(bus =>
{
    bus.SetKebabCaseEndpointNameFormatter();
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
builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticConfiguration:Uri"]))
        {
            IndexFormat = $"{context.Configuration["ApplicationName"]}-logs".Replace(".", "-"),
            AutoRegisterTemplate = true
        })
        .ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddOpenTelemetryTracing(x =>
{
    x.SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("IGroceryStore")
            .AddTelemetrySdk()
            .AddEnvironmentVariableDetector())
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddModulesInstrumentation(modules)
        .AddSource("MassTransit")
        .AddEntityFrameworkCoreInstrumentation()
        .AddNpgsql()
        .AddAWSInstrumentation()
        .AddJaeger();
});

var app = builder.Build();
System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.UseSwagger();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

foreach (var module in modules)
{
    module.Use(app);
}

app.MapGet("/api/health", () => "IGroceryStore is healthy")
    .WithTags(SwaggerTags.HealthChecks);

foreach (var module in modules)
{
    module.Expose(app);
}

app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "IGroceryStore"); });

app.MapFallbackToFile("index.html");

var databaseInitializer = app.Services.GetRequiredService<DbInitializer>();
if (builder.Environment.IsDevelopment() || builder.Environment.IsTestEnvironment())
{
    await databaseInitializer.MigrateWithEnsuredDeletedAsync(moduleAssemblies);
}
else
{
    await databaseInitializer.MigrateAsync(moduleAssemblies);
}
app.Run();
