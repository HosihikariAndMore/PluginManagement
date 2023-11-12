#define WINDOWS_TEST_LLv2

using Hosihikari.PluginManager;

namespace Hosihikari.PluginManagement;

internal static class Main
{

#if WINDOWS_TEST_LLv2

    public delegate void EntryPoint();

    public
#else
    [UnmanagedCallersOnly]
    public 
#endif
    static void Initialize()
    {
        DirectoryInfo directoryInfo = new(AssemblyPlugin.PluginDirectoryPath);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }
        if (!Directory.Exists(AssemblyPlugin.LibraryDirectoryPath))
        {
            Directory.CreateDirectory(AssemblyPlugin.LibraryDirectoryPath);
        }

        LoadPluginsRecursively(directoryInfo);

        foreach (AssemblyPlugin plugin in AssemblyPlugin.s_plugins)
        {
            if (string.IsNullOrWhiteSpace(plugin.Name))
            {
                throw new NullReferenceException();
            }
            Manager.Initialize(plugin.Name);
        }
    }
    private static void LoadPluginsRecursively(DirectoryInfo directoryInfo)
    {
        foreach (FileInfo file in directoryInfo.EnumerateFiles())
        {
            AssemblyHandler handler = new(file);
            if (handler.TryLoad(out var _) && handler.IsPluginAssembly)
            {
                var plugin = handler.CreatePlugin();
                Manager.Load(plugin);
            }
        }
        foreach (DirectoryInfo directory in directoryInfo.EnumerateDirectories())
        {
            LoadPluginsRecursively(directory);
        }
    }
}
