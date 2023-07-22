using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.Loader;

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
        AssemblyLoadContext context = new PluginLoadContext(FileInfo.Name, true);
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
            string path = Path.Combine(
                Path.GetFullPath(PluginManager.LibraryDirectoryPath),
                Path.ChangeExtension(referencedAssembly.Name, ".dll")!);
            if (!File.Exists(path))
            {
                continue;
            }
            context.LoadFromAssemblyPath(path);
        }
        EntryPointAttributeBase? entry =
            Assembly.GetCustomAttribute<EntryPointAttributeBase>();
        if (entry is null)
        {
            context.Unload();
            Console.Error.WriteLine("{0} load failed. (Entry point not found)", FileInfo.Name);
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
