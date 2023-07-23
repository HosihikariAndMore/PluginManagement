namespace Hosihikari.Loader;

public static class PluginManager
{
    public const string PluginDirectoryPath = "plugins";
    public const string LibraryDirectoryPath = "lib";

    private static readonly Dictionary<string, Plugin> s_plugins;

    static PluginManager()
    {
        s_plugins = new();
    }

    public static void Load(Plugin plugin)
    {
        if (!plugin.Load())
        {
            return;
        }
        s_plugins[plugin.FileInfo.Name] = plugin;
    }

    internal static void Initialize(string name)
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
        plugin.Unload();
        s_plugins.Remove(name);
    }

    internal static IEnumerable<string> EnumerateNames()
    {
        foreach (string name in s_plugins.Keys)
        {
            yield return name;
        }
    }
}
