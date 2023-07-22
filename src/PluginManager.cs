using System.Runtime.Loader;

namespace Loader;

internal static class PluginManager
{
    private static readonly Dictionary<string, PluginData> _pluginContexts;
    private static readonly string _pluginDirectoryPath;

    static PluginManager()
    {
        _pluginContexts = new();
        _pluginDirectoryPath = "plugins";
    }

    public static bool Load(FileInfo file)
    {
        AssemblyLoadContext loadContext = new(file.FullName);
        PluginData pluginData = new(file, loadContext);
        if (!pluginData.Load())
        {
            return false;
        }
        _pluginContexts[file.Name] = pluginData;
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

    public static IEnumerable<FileInfo> EnumerableAllFiles()
    {
        DirectoryInfo directoryInfo = new(_pluginDirectoryPath);
        foreach (FileInfo file in directoryInfo.EnumerateFiles())
        {
            yield return file;
        }
    }
}
