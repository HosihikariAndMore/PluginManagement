using System.Runtime.InteropServices;

namespace Hosihikari.Loader;

public static class Main
{
    [UnmanagedCallersOnly]
    public static void Initialize()
    {
        DirectoryInfo directoryInfo = new(PluginManager.PluginDirectoryPath);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        if (!Directory.Exists(PluginManager.LibraryDirectoryPath))
        {
            Directory.CreateDirectory(PluginManager.LibraryDirectoryPath);
        }

        foreach (FileInfo file in directoryInfo.EnumerateFiles())
        {
            AssemblyPlugin plugin = new(file);
            PluginManager.Load(plugin);
        }

        foreach (string name in PluginManager.EnumerateNames())
        {
            PluginManager.Initialize(name);
        }
    }
}
