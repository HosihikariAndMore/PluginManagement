using System.IO;
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

    public static IEnumerable<string> EnumerableAllDll()
    {
        DirectoryInfo directoryInfo = new("plugins");
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        foreach (var file in directoryInfo.EnumerateFiles("*.dll"))
        {
            yield return file.FullName;
        }
    }

    public static void LoadAllPlugin()
    {
        foreach (var file in EnumerableAllDll())
        {
            try
            {
                if (LoadPlugin(file))
                {
                    continue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.Error.WriteLine("{0} load failed.", file);
        }
    }
}
