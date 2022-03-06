using IGroceryStore.Baskets.Core;
using IGroceryStore.Products.Core;
using IGroceryStore.Users.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Worker;


var builder = Host.CreateDefaultBuilder(args);
    
builder.ConfigureServices((hostContext, services) =>
{
    services.AddUsers(hostContext.Configuration);
    services.AddProducts(hostContext.Configuration);
    services.AddBaskets(hostContext.Configuration);
    services.AddHostedService<MessageProcessor>();
});

var app = builder.Build();

app.Run();