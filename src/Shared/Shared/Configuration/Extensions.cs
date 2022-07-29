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


    private static IEnumerable<string> GetSettings(this HostBuilderContext ctx) =>
        Directory.EnumerateFiles(ctx.HostingEnvironment.GetPath(),
            "module.json", SearchOption.AllDirectories);

    private static IEnumerable<string> GetSettings(this HostBuilderContext ctx, string pattern) =>
        Directory.EnumerateFiles(ctx.HostingEnvironment.GetPath(),
            $"module.{pattern}.json", SearchOption.AllDirectories);

    private static string GetPath(this IHostEnvironment env) =>
        env.ContentRootPath.Split(env.ApplicationName).First();
}