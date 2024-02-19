using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.PluginManagement;

internal class PluginLoadContext(FileSystemInfo fileInfo) : AssemblyLoadContext(fileInfo.Name, true)
{
    private static readonly Dictionary<string, Assembly> s_loadedAssembly = [];

    private readonly AssemblyDependencyResolver _resolver = new(fileInfo.FullName);

    protected override Assembly? Load(AssemblyName assemblyName)
    {

        if (s_loadedAssembly.TryGetValue(assemblyName.FullName, out Assembly? assembly))
        {
            return assembly;
        }

        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath is not null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }

        return null;
    }

    public new Assembly LoadFromAssemblyPath(string assemblyPath)
    {
        Assembly assembly = base.LoadFromAssemblyPath(assemblyPath);
        s_loadedAssembly[assembly.GetName().FullName] = assembly;
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