using System.Runtime.InteropServices;

namespace Loader;

public static class Main
{
    [UnmanagedCallersOnly]
    public static void Initialize()
    {
        DirectoryInfo directoryInfo = new("plugins");
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        PluginManager.LoadAllPlugin();
    }
}
