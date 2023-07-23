using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.Loader;

internal static class PluginManager
{
    private static readonly Dictionary<string, Plugin> _pluginContexts;
    public static readonly string PluginDirectoryPath;
    public static readonly string LibraryDirectoryPath;
    public static readonly Dictionary<string, Assembly> LoadedAssembly;

    static PluginManager()
    {
        _pluginContexts = new();
        PluginDirectoryPath = "plugins";
        LibraryDirectoryPath = "lib";

        DirectoryInfo directoryInfo = new(LibraryDirectoryPath);
        Assembly loader = Assembly.GetExecutingAssembly();
        AssemblyLoadContext? context =
            AssemblyLoadContext.GetLoadContext(loader);
        LoadedAssembly = new()
        {
            [loader.GetName().FullName] = loader
        };

        if (context is null)
        {
            return;
        }
        foreach (FileInfo file in directoryInfo.EnumerateFiles())
        {
            Assembly assembly;
            try
            {
                assembly = context.LoadFromAssemblyPath(file.FullName);
            }
            catch (BadImageFormatException)
            {
                continue;
            }
            LoadedAssembly[assembly.GetName().FullName] = assembly;
        }
    }

    public static bool Load(FileInfo file)
    {
        Plugin plugin = new(file);
        if (!plugin.Load() || plugin.Assembly is null || !plugin.Initialize())
        {
            return false;
        }
        _pluginContexts[file.Name] = plugin;
        LoadedAssembly[plugin.Assembly.GetName().FullName] = plugin.Assembly;
        return true;
    }

    public static bool Unload(string name)
    {
        if (!_pluginContexts.TryGetValue(name, out Plugin? plugin))
        {
            return false;
        }
        if (plugin.Assembly is not null)
        {
            LoadedAssembly.Remove(plugin.Assembly.GetName().FullName);
            plugin.Unload();
        }
        _pluginContexts.Remove(name);
        return true;
    }
}
