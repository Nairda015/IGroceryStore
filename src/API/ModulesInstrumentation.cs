using IGroceryStore.Shared.Common;
using OpenTelemetry.Trace;

namespace IGroceryStore.API;

public static class ModulesInstrumentation
{
    public static TracerProviderBuilder AddModulesInstrumentation(this TracerProviderBuilder builder, IEnumerable<IModule> modules)
    {
        foreach (var module in modules)
        {
            builder.AddSource(module.Name);
        }
        
        return builder;
    }
}