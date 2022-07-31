using System.Reflection;
using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace IGroceryStore.Shared.Configuration;

public static class AppInitializer
{
    private const string ModulePrefix = "IGroceryStore";
    public static AppContext Initialize(WebApplicationBuilder builder)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var locations = assemblies.Where(x => !x.IsDynamic).Select(x => x.Location).ToArray();
        var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Where(x => !locations.Contains(x, StringComparer.InvariantCultureIgnoreCase))
            .ToList();

        var moduleAssemblies = new List<Assembly>();
        foreach (var file in files)
        {
            var root = file.Split(ModulePrefix, 2).Last();
            if (!root.Contains(ModulePrefix)) continue;

            var moduleName = root.Split($"{ModulePrefix}")[1]
                .Split(".", StringSplitOptions.RemoveEmptyEntries)[0];
            var enabled = builder.Configuration.GetValue<bool>($"{moduleName}:ModuleEnabled");

            if (!enabled) continue;
            var assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(file));
            assemblies.Add(assembly);
            moduleAssemblies.Add(assembly);
        }

        var modules = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(IModule).IsAssignableFrom(x) && x.IsClass)
            .OrderBy(x => x.Name)
            .Select(Activator.CreateInstance)
            .Cast<IModule>()
            .ToList();
        
        return new AppContext(assemblies.ToList(), moduleAssemblies, modules.ToHashSet());
    }
}