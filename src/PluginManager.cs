using System.Reflection;
using System.Runtime.Loader;

namespace Loader;

internal static class PluginManager
{
    public static Dictionary<AssemblyLoadContext, List<Action>> PluginContexts { get; }

    static PluginManager()
    {
        PluginContexts = new();
    }

    public static bool LoadPlugin(string path)
    {
        AssemblyLoadContext loadContext = new(path);
        Assembly assembly = loadContext.LoadFromAssemblyPath(path);
        bool success = false;
        List<Action> plugins = new();
        foreach (var entryPoint in assembly.GetCustomAttributes<EntryPointAttributeBase>())
        {
            IPlugin plugin = entryPoint.CreateInstance();
            plugin.Initialize();
            success = true;
            plugins.Add(plugin.Unload);
        }
        if (!success)
        {
            loadContext.Unload();
            return false;
        }
        PluginContexts[loadContext] = plugins;
        return true;
    }
}
