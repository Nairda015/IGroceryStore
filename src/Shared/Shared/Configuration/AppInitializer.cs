using System.Reflection;
using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IGroceryStore.Shared.Configuration;

public static class AppInitializer
{
    private const string ModulePrefix = "IGroceryStore";
    public static AppContext Initialize(WebApplicationBuilder builder)
    {
        var disabledModules = new HashSet<string>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var locations = assemblies.Where(x => !x.IsDynamic).Select(x => x.Location).ToArray();
        var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Where(x => !locations.Contains(x, StringComparer.InvariantCultureIgnoreCase))
            .ToList();

        var groceryStoreFiles = files
            .Select(x => x.Split(ModulePrefix, 2).Last())
            .Where(x => x.Contains(ModulePrefix))
            .ToList();
        
        foreach (var file in files)
        {
            var root = file.Split(ModulePrefix, 2).Last();
            if (!root.Contains(ModulePrefix)) continue;

            var moduleName = root.Split($"{ModulePrefix}")[1].Split(".", StringSplitOptions.RemoveEmptyEntries)[0];
            var enabled = builder.Configuration.GetValue<bool>($"{moduleName}:ModuleEnabled");
            
            if (enabled)
            {
                assemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(file)));
            }
            else
            {
                disabledModules.Add(moduleName);
            }
        }

        builder.Services.AddControllers().ConfigureApplicationPartManager(manager =>
        {
            var appParts = disabledModules.SelectMany(x => manager.ApplicationParts
                .Where(part => part.Name.Contains(x, StringComparison.InvariantCultureIgnoreCase))).ToList();

            foreach (var part in appParts)
            {
                manager.ApplicationParts.Remove(part);
            }
                    
            manager.FeatureProviders.Add(new CustomControllerFeatureProvider());
        });

        var modules = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(IModule).IsAssignableFrom(x) && x.IsClass)
            .OrderBy(x => x.Name)
            .Select(Activator.CreateInstance)
            .Cast<IModule>()
            .ToList();
        
        return new (assemblies.ToList(), modules.ToHashSet());
    }
}