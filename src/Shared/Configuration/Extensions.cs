using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IGroceryStore.Shared.Configuration;

public static class Extensions
{
    public static void ConfigureModules(this IHostBuilder builder) =>
        builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            foreach (var settings in ctx.GetSettings())
            {
                cfg.AddJsonFile(settings);
            }

            foreach (var settings in ctx.GetSettings($"{ctx.HostingEnvironment.EnvironmentName}"))
            {
                cfg.AddJsonFile(settings);
            }
        });

    public static void ConfigureEnvironmentVariables(this IHostBuilder builder) =>
        builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            cfg.AddEnvironmentVariables(prefix: "ModuleName_");
        });
    
    private static IEnumerable<string> GetSettings(this HostBuilderContext ctx) =>
        Directory.EnumerateFiles(ctx.HostingEnvironment.GetPath(),
            "modulesettings.json", SearchOption.AllDirectories);

    private static IEnumerable<string> GetSettings(this HostBuilderContext ctx, string pattern) =>
        Directory.EnumerateFiles(ctx.HostingEnvironment.GetPath(),
            $"modulesettings.{pattern}.json", SearchOption.AllDirectories);

    private static string GetPath(this IHostEnvironment env) =>
        env.ContentRootPath.Split("src").First();


    public static Type[] TryGetTypes(this Assembly assembly)
    {
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            types = e.Types!;
        }

        return types;
    }
}
