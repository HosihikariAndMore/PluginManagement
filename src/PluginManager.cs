using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.Loader;

internal static class PluginManager
{
    public const string PluginDirectoryPath = "plugins";
    public const string LibraryDirectoryPath = "lib";

    private static readonly Dictionary<string, Plugin> s_plugins;
    private static readonly Dictionary<string, Assembly> s_loadedAssembly;

    static PluginManager()
    {
        s_plugins = new();
        s_loadedAssembly = new();

        Assembly loader = Assembly.GetExecutingAssembly();
        AssemblyLoadContext? context =
            AssemblyLoadContext.GetLoadContext(loader);
        if (context is null)
        {
            return;
        }

        DirectoryInfo directoryInfo = new(LibraryDirectoryPath);
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
            s_loadedAssembly[assembly.GetName().FullName] = assembly;
        }
    }

    public static void Load(FileInfo file)
    {
        Plugin plugin = new(file);
        if (!plugin.Load() || plugin.Assembly is null)
        {
            return;
        }
        s_plugins[file.Name] = plugin;
        s_loadedAssembly[plugin.Assembly.GetName().FullName] = plugin.Assembly;
    }

    public static void Initialize(string name)
    {
        if (!s_plugins.TryGetValue(name, out Plugin? plugin))
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
        if (!s_plugins.TryGetValue(name, out Plugin? plugin))
        {
            return;
        }
        if (plugin.Assembly is not null)
        {
            s_loadedAssembly.Remove(plugin.Assembly.GetName().FullName);
            plugin.Unload();
        }
        s_plugins.Remove(name);
    }

    public static bool TryGetLoaded(string name, out Assembly? assembly) =>
        s_loadedAssembly.TryGetValue(name, out assembly);

    public static IEnumerable<string> EnumerateNames()
    {
        foreach (string name in s_plugins.Keys)
        {
            yield return name;
        }
    }
}
