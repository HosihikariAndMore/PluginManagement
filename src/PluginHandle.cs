using Hosihikari.PluginManagement;
using System.Runtime.InteropServices;

namespace Hosihikari.PluginManager;

[StructLayout(LayoutKind.Sequential)]
public readonly unsafe struct PluginHandle(Plugin plugin)
{
    public readonly nint Handle = GCHandle.ToIntPtr(GCHandle.Alloc(plugin));
}
