using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.PluginManagement;

internal class PluginLoadContext : AssemblyLoadContext
{
    private static readonly Dictionary<string, Assembly> s_loadedAssembly;

    static PluginLoadContext()
    {
        s_loadedAssembly = [];
    }

    public PluginLoadContext(FileSystemInfo fileInfo) : base(fileInfo.Name, true)
    {
        Unloading += _ =>
        {
            foreach (Assembly assembly in Assemblies)
            {
                s_loadedAssembly.Remove(assembly.GetName().FullName);
            }
        };
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
}