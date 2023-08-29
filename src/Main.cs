using System.Runtime.InteropServices;

namespace Hosihikari.PluginManagement;

internal static class Main
{
    [UnmanagedCallersOnly]
    public static void Initialize()
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
            AssemblyPlugin plugin = new(file);
            Manager.Load(plugin);
        }
        foreach (DirectoryInfo directory in directoryInfo.EnumerateDirectories())
        {
            LoadPluginsRecursively(directory);
        }
    }
}
