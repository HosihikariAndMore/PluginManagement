namespace Hosihikari.PluginManager;

public static class Manager
{
    private static readonly Dictionary<string, Plugin> s_plugins;

    static Manager()
    {
        s_plugins = new();
    }

    public static void Load(Plugin plugin)
    {
        if (!plugin.Load())
        {
            return;
        }
        s_plugins[plugin.Name] = plugin;
    }

    public static void Initialize(string name)
    {
        if (!s_plugins.TryGetValue(name, out Plugin? plugin))
        {
            throw new NullReferenceException();
        }
        if (plugin.Initialize())
        {
            return;
        }
        Unload(name);
    }

    public static void Unload(string name)
    {
        if (!s_plugins.TryGetValue(name, out Plugin? plugin))
        {
            throw new NullReferenceException();
        }
        plugin.Unload();
        s_plugins.Remove(name);
    }

    internal static IEnumerable<string> EnumerateNames() => s_plugins.Keys;
}
