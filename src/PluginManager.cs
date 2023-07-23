using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.Loader;

internal static class PluginManager
{
    public static readonly string PluginDirectoryPath;
    public static readonly string libraryDirectoryPath;

    private static readonly Dictionary<string, Plugin> _plugins;
    private static readonly Dictionary<string, Assembly> _loadedAssembly;

    static PluginManager()
    {
        PluginDirectoryPath = "plugins";
        libraryDirectoryPath = "lib";

        _plugins = new();

        Assembly loader = Assembly.GetExecutingAssembly();
        _loadedAssembly = new();

        AssemblyLoadContext? context =
            AssemblyLoadContext.GetLoadContext(loader);
        if (context is null)
        {
            return;
        }
        DirectoryInfo directoryInfo = new(libraryDirectoryPath);
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
            _loadedAssembly[assembly.GetName().FullName] = assembly;
        }
    }

    public static void Load(FileInfo file)
    {
        Plugin plugin = new(file);
        if (!plugin.Load() || plugin.Assembly is null)
        {
            return;
        }
        _plugins[file.Name] = plugin;
        _loadedAssembly[plugin.Assembly.GetName().FullName] = plugin.Assembly;
    }

    public static void Initialize(string name)
    {
        if (!_plugins.TryGetValue(name, out Plugin? plugin))
        {
            return;
        }
        if (!plugin.Initialize())
        {
            Unload(name);
        }
    }

    public static void Unload(string name)
    {
        if (!_plugins.TryGetValue(name, out Plugin? plugin))
        {
            return;
        }
        if (plugin.Assembly is not null)
        {
            _loadedAssembly.Remove(plugin.Assembly.GetName().FullName);
            plugin.Unload();
        }
        _plugins.Remove(name);
    }

    public static bool TryGetLoaded(string name, out Assembly? assembly) =>
        _loadedAssembly.TryGetValue(name, out assembly);

    public static IEnumerable<string> EnumerateNames()
    {
        foreach (string name in _plugins.Keys)
        {
            yield return name;
        }
    }
}
