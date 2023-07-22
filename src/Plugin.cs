using System.Reflection;
using System.Runtime.Loader;

namespace Loader;

public class Plugin
{
    internal FileInfo FileInfo { get; }
    internal AssemblyLoadContext AssemblyLoadContext { get; }
    public event EventHandler? Unloading;

    internal Plugin(FileInfo file)
    {
        FileInfo = file;
        AssemblyLoadContext = new(FileInfo.Name);
    }

    internal bool Load()
    {
        Assembly assembly;
        try
        {
            assembly =
                AssemblyLoadContext.LoadFromAssemblyPath(FileInfo.FullName);
        }
        catch (BadImageFormatException)
        {
            return false;
        }
        EntryPointAttributeBase? entry =
            assembly.GetCustomAttribute<EntryPointAttributeBase>();
        if (entry is null)
        {
            AssemblyLoadContext.Unload();
            return false;
        }
        IPlugin plugin = entry.CreateInstance();
        plugin.Initialize(this);
        return true;
    }

    internal void Unload()
    {
        if (Unloading is not null)
        {
            Unloading(this, new());
        }
        AssemblyLoadContext.Unload();
    }
}
