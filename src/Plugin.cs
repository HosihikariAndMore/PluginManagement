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
        PluginLoadContext context = new(FileInfo.Name, true);
        try
        {
            Assembly =
                context.LoadFromAssemblyPath(FileInfo.FullName);
        }
        catch (BadImageFormatException)
        {
            return false;
        }
        return true;
    }

    internal bool Initialize()
    {
        if (Assembly is null)
        {
            return false;
        }
        EntryPointAttributeBase? attribute =
            Assembly.GetCustomAttribute<EntryPointAttributeBase>();
        if (attribute is null)
        {
            Unload();
            Console.Error.WriteLine(
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} ERROR] {Assembly.GetName().Name} initialize failed. (Entry point not found)");
            return false;
        }
        IEntryPoint entry = attribute.CreateInstance();
        entry.Initialize(this);
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
            Unloading(this, EventArgs.Empty);
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
