using System.Runtime.InteropServices;

namespace Loader;

public static class Main
{
    [UnmanagedCallersOnly]
    public static void Initialize()
    {
        if (!Directory.Exists("plugins"))
        {
            Directory.CreateDirectory("plugins");
        }
        foreach (FileInfo file in PluginManager.EnumerableAllFiles())
        {
            PluginManager.Load(file);
        }
    }
}
