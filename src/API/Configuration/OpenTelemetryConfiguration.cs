using System.Diagnostics;
using IGroceryStore.Shared.Common;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace IGroceryStore.API.Configuration;

public static class OpenTelemetryConfiguration
{
    public static void ConfigureOpenTelemetry(this WebApplicationBuilder builder, IEnumerable<IModule> modules)
    {
        builder.Services.AddOpenTelemetryTracing(x =>
        {
            x.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("IGroceryStore")
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector())
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddModulesInstrumentation(modules)
                .AddSource("MassTransit")
                .AddEntityFrameworkCoreInstrumentation()
                .AddNpgsql()
                .AddAWSInstrumentation()
                .AddJaeger();
        });
    }
    
    private static TracerProviderBuilder AddModulesInstrumentation(this TracerProviderBuilder builder, IEnumerable<IModule> modules)
    {
        foreach (var module in modules)
        {
            builder.AddSource(module.Name);
        }
        
        return builder;
    }
    
    private static TracerProviderBuilder AddJaeger(this TracerProviderBuilder builder)
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
