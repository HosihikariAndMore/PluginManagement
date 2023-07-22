using System.Runtime.InteropServices;

namespace Loader;

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
        foreach (FileInfo file in directoryInfo.EnumerateFiles())
        {
            PluginManager.Load(file);
        }
    }
}
