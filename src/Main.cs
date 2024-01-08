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
        while (directoryQueue.Count > 0)
        {
            DirectoryInfo directoryInfo = directoryQueue.Dequeue();
            foreach (FileInfo file in directoryInfo.EnumerateFiles())
            {
                AssemblyPlugin plugin = new(file);
                Manager.Load(plugin);
            }

            foreach (DirectoryInfo subdirectory in directoryInfo.EnumerateDirectories())
            {
                directoryQueue.Enqueue(subdirectory);
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