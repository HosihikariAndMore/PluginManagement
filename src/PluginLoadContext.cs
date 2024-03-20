using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.PluginManagement;

internal class PluginLoadContext : AssemblyLoadContext
{
    private static readonly Dictionary<string, Assembly> s_loadedAssembly = [];

    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(FileSystemInfo fileInfo) : base(fileInfo.Name, true)
    {
        _resolver = new(fileInfo.FullName);
        Unloading += _ =>
        {
            foreach (Assembly assembly in Assemblies)
            {
                s_loadedAssembly.Remove(assembly.GetName().FullName);
            }
        };
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (s_loadedAssembly.TryGetValue(assemblyName.FullName, out Assembly? assembly))
        {
            return assembly;
        }

        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath is not null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    public new Assembly LoadFromAssemblyPath(string assemblyPath)
    {
        Assembly assembly = base.LoadFromAssemblyPath(assemblyPath);
        s_loadedAssembly[assembly.GetName().FullName] = assembly;
        return assembly;
    }
}