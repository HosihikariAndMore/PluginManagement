using Hosihikari.PluginManager;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Hosihikari.PluginManagement;

internal unsafe static class Main
{
    [UnmanagedCallersOnly]
    public static void Initialize()
    {
        try
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
                    LoadPlugin(file);
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
        catch (Exception ex)
        {
            Environment.FailFast(default, ex);
        }
    }

    public static AssemblyPlugin LoadPlugin(FileInfo file)
    {
        AssemblyPlugin plugin = new(file);
        Manager.Load(plugin);
        return plugin;
    }

    [UnmanagedCallersOnly]
    public static unsafe void LoadPluginUnmanaged(byte* pathStr, void** handle, void* arg, delegate* unmanaged[Stdcall]<void*, /* bool */bool, /* char const* */byte*, void> fptr)
    {
        try
        {
            var path = Utf8StringMarshaller.ConvertToManaged(pathStr);
            if (string.IsNullOrWhiteSpace(path))
                throw new NullReferenceException("Path is null or whitespace.");
            var temp = new PluginHandle(LoadPlugin(new FileInfo(path)));
            *handle = (void*)temp.Handle;
            fptr(arg, true, null);
        }
        catch (Exception ex)
        {
            var str = Utf8StringMarshaller.ConvertToUnmanaged(ex.ToString());
            fptr(arg, false, str);
            Utf8StringMarshaller.Free(str);
        }
    }

    [UnmanagedCallersOnly]
    public unsafe static void Load(void* handle, void* arg, delegate* unmanaged[Stdcall]<void*, /* bool */bool, /* char const* */byte*, void> fptr)
    {
        try
        {
            var target = GCHandle.FromIntPtr((nint)handle).Target as Plugin
            ?? throw new NullReferenceException("Plugin is null");
            target.Load();
            target.Initialize();

            fptr(arg, true, null);
            return;
        }
        catch (Exception ex)
        {
            var str = Utf8StringMarshaller.ConvertToUnmanaged(ex.ToString());
            fptr(arg, false, str);
            Utf8StringMarshaller.Free(str);
            return;
        }
    }

    [UnmanagedCallersOnly]
    public unsafe static void Enable(void* handle, void* arg, delegate* unmanaged[Stdcall]<void*, /* bool */bool, /* char const* */byte*, void> fptr)
    {
        fptr(arg, true, null);
        return;
    }

    [UnmanagedCallersOnly]
    public unsafe static void Disable(void* handle, void* arg, delegate* unmanaged[Stdcall]<void*, /* bool */bool, /* char const* */byte*, void> fptr)
    {
        fptr(arg, true, null);
        return;
    }

    [UnmanagedCallersOnly]
    public unsafe static void Unload(void* handle, void* arg, delegate* unmanaged[Stdcall]<void*, /* bool */bool, /* char const* */byte*, void> fptr)
    {
        try
        {
            var target = GCHandle.FromIntPtr((nint)handle).Target as Plugin
            ?? throw new NullReferenceException("Plugin is null");
            target.Unload();
            fptr(arg, true, null);
            return;
        }
        catch (Exception ex)
        {
            var str = Utf8StringMarshaller.ConvertToUnmanaged(ex.ToString());
            fptr(arg, false, str);
            Utf8StringMarshaller.Free(str);
            return;
        }
    }

    [UnmanagedCallersOnly]
    public unsafe static void ReleaseHandle(void* handle, void* arg, delegate* unmanaged[Stdcall]<void*, /* bool */bool, /* char const* */byte*, void> fptr)
    {
        var gch = GCHandle.FromIntPtr((nint)handle);
        if (gch.IsAllocated)
            gch.Free();
        fptr(arg, true, null);
        return;
    }
}