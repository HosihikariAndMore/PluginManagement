using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace Hosihikari.PluginManagement;

internal static class Main
{
    [UnmanagedCallersOnly]
    public static void Initialize()
    {
        DirectoryInfo pluginsDirectory = new(AssemblyPlugin.PluginDirectoryPath);
        if (!pluginsDirectory.Exists)
        {
            return;
        }

        Queue<DirectoryInfo> directoryQueue = new();
        directoryQueue.Enqueue(pluginsDirectory);
        while (directoryQueue.TryDequeue(out DirectoryInfo? directoryInfo))
        {
            foreach (DirectoryInfo subdirectory in directoryInfo.EnumerateDirectories())
            {
                directoryQueue.Enqueue(subdirectory);
            }

            foreach (FileInfo file in directoryInfo.EnumerateFiles())
            {
                AssemblyPlugin plugin = new(file);
                Manager.Load(plugin);
            }
        }

        foreach (string pluginName in (from plugin in AssemblyPlugin.Plugins select plugin.Name).ToImmutableArray())
        {
            if (string.IsNullOrWhiteSpace(pluginName))
            {
                throw new NullReferenceException();
            }

            Manager.Initialize(pluginName);
        }
    }
}