using System.Runtime.InteropServices;

namespace Hosihikari.PluginManagement;

[StructLayout(LayoutKind.Sequential)]
internal readonly unsafe struct PluginHandle(Plugin plugin)
{
    public readonly nint Handle = GCHandle.ToIntPtr(GCHandle.Alloc(plugin));
}