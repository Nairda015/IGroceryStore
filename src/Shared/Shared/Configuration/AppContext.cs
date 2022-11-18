using System.Reflection;
using IGroceryStore.Shared.Common;

namespace IGroceryStore.Shared.Configuration;

public sealed record AppContext(List<Assembly> LoadedAssemblies, List<Assembly> ModuleAssemblies, HashSet<IModule> LoadedModules);