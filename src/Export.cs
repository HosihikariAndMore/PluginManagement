using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Hosihikari.PluginManagement;

internal unsafe static class Export
{
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
            string? path = Utf8StringMarshaller.ConvertToManaged(pathStr);
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new NullReferenceException("Path is null or whitespace.");
            }

            PluginHandle temp = new(LoadPlugin(new FileInfo(path)));
            *handle = (void*)temp.Handle;
            fptr(arg, true, null);
        }
        catch (Exception ex)
        {
            byte* str = Utf8StringMarshaller.ConvertToUnmanaged(ex.ToString());
            fptr(arg, false, str);
            Utf8StringMarshaller.Free(str);
        }
    }

    [UnmanagedCallersOnly]
    public unsafe static void Load(void* handle, void* arg, delegate* unmanaged[Stdcall]<void*, /* bool */bool, /* char const* */byte*, void> fptr)
    {
        try
        {
            Plugin target = GCHandle.FromIntPtr((nint)handle).Target as Plugin ?? throw new NullReferenceException("Plugin is null");
            Manager.Initialize(target.Name);
            fptr(arg, true, null);
        }
        catch (Exception ex)
        {
            byte* str = Utf8StringMarshaller.ConvertToUnmanaged(ex.ToString());
            fptr(arg, false, str);
            Utf8StringMarshaller.Free(str);
        }
    }

    [UnmanagedCallersOnly]
    public unsafe static void Enable(void* handle, void* arg, delegate* unmanaged[Stdcall]<void*, /* bool */bool, /* char const* */byte*, void> fptr)
    {
        fptr(arg, true, null);
    }

    [UnmanagedCallersOnly]
    public unsafe static void Disable(void* handle, void* arg, delegate* unmanaged[Stdcall]<void*, /* bool */bool, /* char const* */byte*, void> fptr)
    {
        fptr(arg, true, null);
    }

    [UnmanagedCallersOnly]
    public unsafe static void Unload(void* handle, void* arg, delegate* unmanaged[Stdcall]<void*, /* bool */bool, /* char const* */byte*, void> fptr)
    {
        try
        {
            Plugin target = GCHandle.FromIntPtr((nint)handle).Target as Plugin ?? throw new NullReferenceException("Plugin is null");
            Manager.Unload(target.Name);
            fptr(arg, true, null);
        }
        catch (Exception ex)
        {
            byte* str = Utf8StringMarshaller.ConvertToUnmanaged(ex.ToString());
            fptr(arg, false, str);
            Utf8StringMarshaller.Free(str);
        }
    }

    [UnmanagedCallersOnly]
    public unsafe static void ReleaseHandle(void* handle, void* arg, delegate* unmanaged[Stdcall]<void*, /* bool */bool, /* char const* */byte*, void> fptr)
    {
        GCHandle gch = GCHandle.FromIntPtr((nint)handle);
        if (gch.IsAllocated)
        {
            gch.Free();
        }

        fptr(arg, true, null);
    }
}