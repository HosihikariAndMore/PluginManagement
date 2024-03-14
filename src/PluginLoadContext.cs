using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.PluginManagement;

internal class PluginLoadContext(FileSystemInfo fileInfo) : AssemblyLoadContext(fileInfo.Name, true)
{
    private static readonly Dictionary<string, Assembly> s_loadedAssembly;

    static PluginLoadContext()
    {
        s_loadedAssembly = [];
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        if (s_loadedAssembly.TryGetValue(assemblyName.FullName, out Assembly? assembly))
        {
            return assembly;
        }

        assembly = Default.LoadFromAssemblyName(assemblyName);
        s_loadedAssembly[assemblyName.FullName] = assembly;
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