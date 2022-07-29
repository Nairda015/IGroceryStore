using DotNetCore.CAP;
using IGroceryStore.Middlewares;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Services;
using Microsoft.AspNetCore.Mvc.Controllers;
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
builder.Services.AddSwaggerGen(c =>
    {
        c.TagActionsBy(api =>
        {
            if (api.GroupName != null)
            {
                return new[] { api.GroupName };
            }

            if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                return new[] { controllerActionDescriptor.ControllerName };
            }

            throw new InvalidOperationException("Unable to determine tag for endpoint.");
        });

        c.DocInclusionPredicate((name, api) => true);
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
//builder.Services.AddHostedService<DbInitializer>();

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

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole()
        .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
    loggingBuilder.AddDebug();
});


var app = builder.Build();


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
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "IGroceryStore"); });


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

app.MapFallbackToFile("index.html");
app.Run();