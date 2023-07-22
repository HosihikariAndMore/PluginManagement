namespace Hosihikari.Loader;

internal static class PluginManager
{
    private static readonly Dictionary<string, Plugin> _pluginContexts;
    public static readonly string PluginDirectoryPath;
    public static readonly string LibraryDirectoryPath;

    static PluginManager()
    {
        _pluginContexts = new();
        PluginDirectoryPath = "plugins";
        LibraryDirectoryPath = "lib";
    }

    public static bool Load(FileInfo file)
    {
        Plugin plugin = new(file);
        if (!plugin.Load())
        {
            return false;
        }
        _pluginContexts[file.Name] = plugin;
        return true;
    }

    public static bool Unload(string name)
    {
        if (!_pluginContexts.ContainsKey(name))
        {
            return false;
        }
        _pluginContexts[name].Unload();
        _pluginContexts.Remove(name);
        return true;
    }
}
