using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.Loader;

internal class PluginLoadContext : AssemblyLoadContext
{
    public PluginLoadContext(string? name, bool isCollectible = false)
        : base(name, isCollectible)
    {
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (PluginManager.LoadedAssembly.TryGetValue(assemblyName.FullName,
            out Assembly? assembly))
        {
            return assembly;
        }
        return base.Load(assemblyName);
    }
}
