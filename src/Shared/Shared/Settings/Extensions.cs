using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Shared.Settings;

public static class Extensions
{
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration)
        where TOptions : ISettings, new()
    {
        var options = new TOptions();
        configuration.GetSection(TOptions.SectionName).Bind(options);
        return options;
    }

    public static void RegisterOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class, ISettings
    {
        var section = configuration.GetSection(TOptions.SectionName);
        services.Configure<TOptions>(section);
    }
}