using FluentValidation;
using IGroceryStore.API.Configuration;
using IGroceryStore.API.Middlewares;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Services;
using IGroceryStore.Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureModules();
builder.Host.ConfigureEnvironmentVariables();
var (assemblies, moduleAssemblies, modules) = AppInitializer.Initialize(builder);

foreach (var module in modules)
{
    module.Register(builder.Services, builder.Configuration);
}

builder.ConfigureSystemManager();
builder.ConfigureLogging();
builder.ConfigureAuthentication();
builder.ConfigureMassTransit();
builder.ConfigureSwagger();
builder.ConfigureOpenTelemetry(modules);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ISnowflakeService, SnowflakeService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services.AddValidatorsFromAssemblies(moduleAssemblies, includeInternalTypes: true);

//**********************************//
var app = builder.Build();
//**********************************//

app.UseSwagger();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
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
    .WithTags(Constants.SwaggerTags.HealthChecks);

foreach (var module in modules)
{
    module.Expose(app);
}

app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "IGroceryStore"); });

app.MapFallbackToFile("index.html");

//TODO: uncomment
// var databaseInitializer = app.Services.GetRequiredService<PostgresInitializer>();
// if (builder.Environment.IsDevelopment() || builder.Environment.IsTestEnvironment())
// {
//     await databaseInitializer.MigrateWithEnsuredDeletedAsync(moduleAssemblies);
// }

app.Run();
