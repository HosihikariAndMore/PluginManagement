namespace Hosihikari.PluginManagement;

public static class Manager
{
    private static readonly Dictionary<string, Plugin> s_plugins;

    static Manager()
    {
        s_plugins = [];
    }

    public static void Load(Plugin plugin)
    {
        try
        {
            plugin.Load();
        }
        catch (BadImageFormatException)
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

        try
        {
            plugin.Initialize();
        }
        catch (EntryPointNotFoundException ex)
        {
            Console.Error.WriteLine(
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} ERROR] {plugin.Name} initialization failed: {ex.Message}");
        }
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

    public static bool Loaded(string name)
    {
        return s_plugins.ContainsKey(name);
    }
}