using IGroceryStore.Shared.Settings;
using MassTransit;

namespace IGroceryStore.API.Configuration;

public static class MassTransitConfiguration
{
    static bool? _isRunningInContainer;

    public static bool IsRunningInContainer =>
        _isRunningInContainer ??= bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var inContainer) && inContainer;

    public static void ConfigureMassTransit(this IBusRegistrationConfigurator configurator, Action<IRabbitMqBusFactoryConfigurator> configure = null)
    {
        configurator.AddDelayedMessageScheduler();
        configurator.SetKebabCaseEndpointNameFormatter();

        configurator.UsingRabbitMq((context, cfg) =>
        {
            if (IsRunningInContainer)
                cfg.Host("rabbitmq");

            cfg.UseDelayedMessageScheduler();

            configure?.Invoke(cfg);

            cfg.ConfigureEndpoints(context);
        });
    }

    public static void ConfigureMassTransit(this WebApplicationBuilder builder)
    {
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
    }
}
