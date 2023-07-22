using System.Reflection;
using System.Runtime.Loader;

namespace Loader;

public class Plugin
{
    internal FileInfo FileInfo { get; }
    internal Assembly? Assembly { get; private set; }

    public event EventHandler? Unloading;

    internal Plugin(FileInfo file)
    {
        FileInfo = file;
    }

    internal bool Load()
    {
        AssemblyLoadContext context = new(FileInfo.Name);
        try
        {
            Assembly =
                context.LoadFromAssemblyPath(FileInfo.FullName);
        }
        catch (BadImageFormatException)
        {
            return false;
        }
        foreach (AssemblyName referencedAssembly in Assembly.GetReferencedAssemblies())
        {
            context.LoadFromAssemblyPath(Path.Combine("lib",
                Path.ChangeExtension(referencedAssembly.Name, ".dll")));
        }
        EntryPointAttributeBase? entry =
            Assembly.GetCustomAttribute<EntryPointAttributeBase>();
        if (entry is null)
        {
            context.Unload();
            return false;
        }
        IPlugin plugin = entry.CreateInstance();
        plugin.Initialize(this);
        return true;
    }

    internal void Unload()
    {
        if (Assembly is null)
        {
            return;
        }
        if (Unloading is not null)
        {
            Unloading(this, new());
        }
        AssemblyLoadContext? context =
            AssemblyLoadContext.GetLoadContext(Assembly);
        if (context is null)
        {
            return;
        }
        context.Unload();
    }
}
