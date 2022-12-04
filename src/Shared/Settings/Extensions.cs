using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IGroceryStore.Shared.Settings;

public static class Extensions
{
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration)
        where TOptions : SettingsBase<TOptions>, ISettings, new()
    {
        var options = new TOptions();
        configuration.GetSection(TOptions.SectionName).Bind(options);
        options.ValidateAndThrow(options);
        return options;
    }

    public static void RegisterOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : SettingsBase<TOptions>, ISettings, new()
    {
        services.AddSingleton<IValidateOptions<TOptions>, TOptions>();
        var section = configuration.GetSection(TOptions.SectionName);
        services.AddOptions<TOptions>()
            .Bind(section)
            .ValidateOnStart();
    }
}
