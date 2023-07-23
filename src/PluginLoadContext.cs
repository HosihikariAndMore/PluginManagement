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
        Assembly loader = Assembly.GetExecutingAssembly();
        AssemblyLoadContext? context = GetLoadContext(loader);
        if (context is not null)
        {
            foreach (Assembly assembly in context.Assemblies)
            {
                if (assembly.GetName().FullName == assemblyName.FullName)
                {
                    return assembly;
                }
            }
        }
        return base.Load(assemblyName);
    }
}
