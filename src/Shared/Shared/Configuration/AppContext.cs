using System.Reflection;
using IGroceryStore.Shared.Abstraction.Common;

namespace IGroceryStore.Shared.Configuration;

public sealed record AppContext(List<Assembly> LoadedAssemblies, HashSet<IModule> LoadedModules);