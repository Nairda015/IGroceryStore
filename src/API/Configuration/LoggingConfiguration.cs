using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace IGroceryStore.API.Configuration;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticConfiguration:Uri"]))
                {
                    IndexFormat = $"{context.Configuration["ApplicationName"]}-logs".Replace(".", "-"),
                    AutoRegisterTemplate = true
                })
                .ReadFrom.Configuration(context.Configuration);
        });
    }
}
