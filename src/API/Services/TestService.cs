using System.Reflection;
using IGroceryStore.Shared.Configuration;
using Serilog;

namespace IGroceryStore.API.Services;

public class TestService
{
    private readonly ILogger<TestService> _logger;

    public TestService(ILogger<TestService> logger)
    {
        _logger = logger;
    }

    public void LogAppContextInfo()
    {
        var context = LogInfo.Context;

        _logger.LogInformation("Types count: {count}", context.LoadedAssemblies.Select(TryGetTypes).Count());
        _logger.LogInformation("Types count: {count}", context.ModuleAssemblies.Select(TryGetTypes).Count());
        _logger.LogInformation("Modules loaded: {modules}", context.LoadedModules.Select(x => x.Name));

        var files = AppInitializer.Files;

        foreach (var file in files)
        {
            _logger.LogInformation("File path {path}", file);
        }
    }
    
    private static Type[] TryGetTypes(Assembly assembly)
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