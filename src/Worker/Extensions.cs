using System.Diagnostics;
using MassTransit;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace IGroceryStore.Worker;

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
    
    public static TracerProviderBuilder AddJaeger(this TracerProviderBuilder builder)
    {
        return builder.AddJaegerExporter(o =>
        {
            o.AgentHost = /*Extensions.IsRunningInContainer ? "jaeger" : */"localhost";
            o.AgentPort = 6831;
            o.MaxPayloadSizeInBytes = 4096;
            o.ExportProcessorType = ExportProcessorType.Batch;
            o.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>
            {
                MaxQueueSize = 2048,
                ScheduledDelayMilliseconds = 5000,
                ExporterTimeoutMilliseconds = 30000,
                MaxExportBatchSize = 512,
            };
        });
    }
}