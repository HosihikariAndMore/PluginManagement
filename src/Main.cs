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
        if (!Directory.Exists(PluginManager.libraryDirectoryPath))
        {
            Directory.CreateDirectory(PluginManager.libraryDirectoryPath);
        }

        foreach (FileInfo file in directoryInfo.EnumerateFiles())
        {
            PluginManager.Load(file);
        }

        foreach (string name in PluginManager.EnumerateNames())
        {
            PluginManager.Initialize(name);
        }
    }
}
