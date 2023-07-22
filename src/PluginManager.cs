using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace Loader;

internal static class PluginManager
{
    public static List<(AssemblyLoadContext ctx, List<IPlugin> plugin)> PluginContexts { get; }

    static PluginManager()
    {
        PluginContexts = new();
    }

    public static bool LoadPlugin(string path)
    {
        AssemblyLoadContext loadContext = new(path);
        Assembly assembly = loadContext.LoadFromAssemblyPath(path);
        var success = false;
        var plugins = new List<IPlugin>();
        foreach (var entryPoint in assembly.GetCustomAttributes<EntryPointAttributeBase>())
        {
            var plugin = entryPoint.CreateInstance();
            plugin.Initialize();
            success = true;
            plugins.Add(plugin);
        }
        PluginContexts.Add((loadContext, plugins));
        return success;
    }
}
