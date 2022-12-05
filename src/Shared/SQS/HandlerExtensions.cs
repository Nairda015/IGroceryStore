using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Shared.SQS;

public static class HandlerExtensions
{
    public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
    {
        var handlers = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(x => typeof(IMessageHandler).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

        foreach (var handler in handlers)
        {
            var handlerType = handler.AsType();
            services.AddScoped(handlerType);
        }

        return services;
    }
}
