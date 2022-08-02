using IGroceryStore.Middlewares;
using IGroceryStore.Services;
using IGroceryStore.Settings;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Services;
using Microsoft.EntityFrameworkCore;
using IGroceryStore.Shared.Configuration;
using IGroceryStore.Shared.Options;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureModules();

var (assemblies, moduleAssemblies, modules) = AppInitializer.Initialize(builder);

foreach (var module in modules)
{
    module.Register(builder.Services, builder.Configuration);
}

//AWS
if (!builder.Environment.IsDevelopment())
{
    builder.Configuration.AddSystemsManager("/Production/IGroceryStore", TimeSpan.FromSeconds(30));
}

//Db
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<DbInitializer>();
}

//Middlewares
builder.Services.AddScoped<ExceptionMiddleware>();

//Messaging
var rabbitSettings = builder.Configuration.GetOptions<RabbitSettings>("Rabbit");
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

var app = builder.Build();
System.AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();

app.UseHttpsRedirection();
app.UseRouting();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

foreach (var module in modules)
{
    module.Use(app);
}

app.MapGet("/", () => "Hello From IGroceryStore");
foreach (var module in modules)
{
    module.Expose(app);
}
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "IGroceryStore"); });

app.MapFallbackToFile("index.html");
app.Run();