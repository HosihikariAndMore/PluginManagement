using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace Loader;

internal static class PluginManager
{
    public static List<(AssemblyLoadContext ctx, IPlugin plugin)> PluginContexts { get; }

    static PluginManager()
    {
        PluginContexts = new();
    }

    public static bool LoadPlugin(string path)
    {
        AssemblyLoadContext loadContext = new(path);
        Assembly assembly = loadContext.LoadFromAssemblyPath(path);
        var success = false;
        foreach (var entryPoint in assembly.GetCustomAttributes<EntryPointAttributeBase>())
        {
            var plugin = entryPoint.CreateInstance();
            plugin.Initialize();
            PluginContexts.Add((loadContext, plugin));
            success = true;
        }
        return success;
    }
}
