using IGroceryStore.Baskets.Core;
using IGroceryStore.Shared;
using IGroceryStore.Shared.Abstraction.Services;
using IGroceryStore.Shared.Services;
using IGroceryStore.Users.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Db
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


// Auth
// builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//     .AddEntityFrameworkStores<ApplicationDbContext>();
//
// builder.Services.AddIdentityServer()
//     .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();


// Services
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

builder.Services.AddShared();
builder.Services.AddBaskets(builder.Configuration);
builder.Services.AddUsers(builder.Configuration);

builder.Services.AddLogging(loggingBuilder => {
    loggingBuilder.AddConsole()
        .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
    loggingBuilder.AddDebug();
});


// MVC/Razor
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();



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

app.UseSwagger(x => x.RouteTemplate = "docs/{documentName}/swagger.json");
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseShared();
//app.UseAuthentication();
//app.UseIdentityServer();
//app.UseAuthorization();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller}/{action=Index}/{id?}");
// app.MapRazorPages();

app.UseEndpoints(x => x.MapControllers());

app.MapFallbackToFile("index.html");

app.Run();