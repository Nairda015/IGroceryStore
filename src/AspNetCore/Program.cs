using DotNetCore.CAP;
using IGroceryStore.Middlewares;
using IGroceryStore.Services;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Services;
using Microsoft.EntityFrameworkCore;
using IGroceryStore.Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureModules();

var (assemblies, modules) = AppInitializer.Initialize(builder);

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
builder.Services.AddCap(options =>
{
    options.UseInMemoryStorage();
    options.UseRabbitMQ("localhost");
    options.UseDashboard();
    //options.ConsumerThreadCount = 0;
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
app.UseCapDashboard();
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