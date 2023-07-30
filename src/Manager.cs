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
        try
        {
            plugin.Load();
        }
        catch (BadImageFormatException)
        {
        }
        s_plugins[plugin.Name] = plugin;
    }

    public static void Initialize(string name)
    {
        if (!s_plugins.TryGetValue(name, out Plugin? plugin))
        {
            throw new NullReferenceException();
        }
        try
        {
            plugin.Initialize();
            return;
        }
        catch (EntryPointNotFoundException)
        {
            Console.Error.WriteLine(
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} ERROR] {plugin.Name} initialize failed. (Entry point not found)"
            );
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
