// using IGroceryStore.Baskets.Core;
// using IGroceryStore.Products.Core;
// using IGroceryStore.Shared.Abstraction.Services;
// using IGroceryStore.Shared.Services;
// using IGroceryStore.Users.Core;
//using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//
 var builder = Host.CreateDefaultBuilder(args);
//     
// builder.ConfigureServices((hostContext, services) =>
// {
//     services.AddUsers(hostContext.Configuration);
//     services.AddProducts(hostContext.Configuration);
//     services.AddBaskets(hostContext.Configuration);
//     services.AddHttpContextAccessor();
//     services.AddSingleton<ICurrentUserService, CurrentUserService>();
//     
//     services.AddCap(options =>
//     {
//         options.UseInMemoryStorage();
//         options.UseRabbitMQ("localhost");
//     });
// });
//
var app = builder.Build();

app.Run();