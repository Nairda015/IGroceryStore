using System.Reflection;
using IGroceryStore.Shared.Abstraction.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace IGroceryStore.Shared.Configuration;

public static class AppInitializer
{
    public static List<string> Files { get; private set; }
    private const string ModulePrefix = "IGroceryStore.";
    public static AppContext Initialize(WebApplicationBuilder builder)
    {
        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .DistinctBy(x => x.Location)
            .ToDictionary(x => x.Location);

        var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll").ToList();

        Files = files;
        var moduleAssemblies = new List<Assembly>();
        foreach (var file in files)
        {
            //Example path
            //IGroceryStore/IGroceryStore/tests/Users/Users.IntegrationTests/bin/Debug/net7.0/IGroceryStore.Products.Contracts.dll
            //IGroceryStore/tests/Users/Users.IntegrationTests/bin/Release/net7.0/IGroceryStore.Shared.Abstraction.dll
            //IGroceryStore/src/API/bin/Release/net7.0/IGroceryStore.Shops.Core.dll
            if (!file.Contains(ModulePrefix)) continue;

            var moduleName = file.Split("/")
                .Last()
                .Split(".", StringSplitOptions.RemoveEmptyEntries)[1];
            var enabled = builder.Configuration.GetValue<bool>($"{moduleName}:ModuleEnabled");

            if (!enabled) continue;
            var assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(file));
            assemblies.TryAdd(assembly.Location, assembly);
            moduleAssemblies.Add(assembly);
        }

        var modules = assemblies
            .SelectMany(x => x.Value.TryGetTypes())
            .Where(x => typeof(IModule).IsAssignableFrom(x) && x.IsClass)
            .OrderBy(x => x.Name)
            .Select(Activator.CreateInstance)
            .Cast<IModule>()
            .ToList();

        return new AppContext(assemblies.Select(x => x.Value).ToList(), moduleAssemblies, modules.ToHashSet());
    }

    private static Type[] TryGetTypes(this Assembly assembly)
    {
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            Log.Error("Failed to load types from assembly {FullName}", assembly.FullName);
            types = e.Types!;
            Log.Error("Found {Length} types in ReflectionTypeLoadException with assembly {FullName}", types.Length, assembly.FullName);
        }
        return types;
    }
}