using MassTransit;

namespace IGroceryStore;

public static class Extensions
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
}