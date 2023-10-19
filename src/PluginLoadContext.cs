using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.PluginManagement;

internal class PluginLoadContext : AssemblyLoadContext
{
    private static readonly Dictionary<string, Assembly> s_loadedAssembly;

    static PluginLoadContext()
    {
        s_loadedAssembly = new();
    }

    public PluginLoadContext(string? name, bool isCollectible = true)
        : base(name, isCollectible)
    {
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (!s_loadedAssembly.TryGetValue(assemblyName.FullName,
            out Assembly? assembly))
        {
            assembly = Default.LoadFromAssemblyName(assemblyName);
        }
        if (assembly is not null)
        {
            s_loadedAssembly[assemblyName.FullName] = assembly;
        }
        return assembly;
    }

    public new void Unload()
    {
        foreach (Assembly assembly in Assemblies)
        {
            s_loadedAssembly.Remove(assembly.GetName().FullName);
        }
        base.Unload();
    }
}
