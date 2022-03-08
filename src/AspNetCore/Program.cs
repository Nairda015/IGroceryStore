using System.Text;
using IGroceryStore.Baskets.Core;
using IGroceryStore.Products.Core;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Abstraction.Constants;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Options;
using IGroceryStore.Shared.Services;
using IGroceryStore.Users.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var envName = builder.Environment.EnvironmentName;

//AWS
builder.Configuration.AddSystemsManager("/Production/IGroceryStore", TimeSpan.FromSeconds(5));

//Config
var userSection = builder.Configuration.GetSection("Users:JwtSettings");
builder.Services.Configure<JwtSettings>(userSection);
var jwtSettings = userSection.Get<JwtSettings>();

//Db
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Auth
var authenticationBuilder = builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwtBearerOptions => Options(jwtBearerOptions, Tokens.Audience.Access))
.AddJwtBearer(Tokens.Audience.Refresh, jwtBearerOptions => Options(jwtBearerOptions, Tokens.Audience.Refresh));

//Services
builder.Services.AddSwaggerGen(c => {
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
}
);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

builder.Services.AddShared();
builder.Services.AddBaskets(builder.Configuration);
builder.Services.AddUsers(builder.Configuration);
builder.Services.AddProducts(builder.Configuration);

builder.Services.AddLogging(loggingBuilder => {
    loggingBuilder.AddConsole()
        .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
    loggingBuilder.AddDebug();
});


// MVC/Razor
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
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
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IGroceryStore");
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseShared();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseEndpoints(x => x.MapControllers());
app.MapGet("/", () => "HelloFromIGroceryStore");
app.MapFallbackToFile("index.html");

app.Run();

JwtBearerOptions Options(JwtBearerOptions jwtBearerOptions, string audience)
{
    jwtBearerOptions.RequireHttpsMetadata = false;
    jwtBearerOptions.SaveToken = true;
    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key)),
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true, 
        ClockSkew = TimeSpan.FromSeconds(jwtSettings.ClockSkew)
    };
    if (audience == Tokens.Audience.Access)
    {
        jwtBearerOptions.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    }
    return jwtBearerOptions;
}