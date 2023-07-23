using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.Loader;

internal class PluginLoadContext : AssemblyLoadContext
{
    public PluginLoadContext(string? name, bool isCollectible = false)
        : base(name, isCollectible)
    {
    }

    protected override Assembly? Load(AssemblyName assemblyName) =>
        AssemblyPlugin.TryGetLoaded(assemblyName.FullName,
            out Assembly? assembly)
            ? assembly
            : base.Load(assemblyName);
}
