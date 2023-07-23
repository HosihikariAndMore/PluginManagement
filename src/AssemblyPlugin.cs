using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.Loader;

public sealed class AssemblyPlugin : Plugin
{
    private static readonly Dictionary<string, Assembly> s_loadedAssembly;
    private Assembly? _assembly;

    public event EventHandler? Unloading;

    static AssemblyPlugin()
    {
        s_loadedAssembly = new();

        Assembly loader = Assembly.GetExecutingAssembly();
        AssemblyLoadContext? context =
            AssemblyLoadContext.GetLoadContext(loader);
        if (context is null)
        {
            return;
        }

        DirectoryInfo directoryInfo = new(PluginManager.LibraryDirectoryPath);
        foreach (FileInfo file in directoryInfo.EnumerateFiles())
        {
            Assembly assembly;
            try
            {
                assembly = context.LoadFromAssemblyPath(file.FullName);
            }
            catch (BadImageFormatException)
            {
                continue;
            }
            s_loadedAssembly[assembly.GetName().FullName] = assembly;
        }
    }

    internal AssemblyPlugin(FileInfo file) : base(file)
    {
    }

    protected internal override bool Load()
    {
        PluginLoadContext context = new(FileInfo.Name, true);
        try
        {
            _assembly =
                context.LoadFromAssemblyPath(FileInfo.FullName);
        }
        catch (BadImageFormatException)
        {
            return false;
        }
        s_loadedAssembly[_assembly.GetName().FullName] = _assembly;
        return true;
    }

    protected internal override bool Initialize()
    {
        if (_assembly is null)
        {
            return false;
        }
        EntryPointAttributeBase? attribute =
            _assembly.GetCustomAttribute<EntryPointAttributeBase>();
        if (attribute is null)
        {
            Unload();
            Console.Error.WriteLine(
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} ERROR] {_assembly.GetName().Name} initialize failed. (Entry point not found)");
            return false;
        }
        IEntryPoint entry = attribute.CreateInstance();
        entry.Initialize(this);
        return true;
    }

    protected internal override void Unload()
    {
        if (_assembly is null)
        {
            return;
        }
        if (Unloading is not null)
        {
            Unloading(this, EventArgs.Empty);
        }
        AssemblyLoadContext? context =
            AssemblyLoadContext.GetLoadContext(_assembly);
        if (context is null)
        {
            return;
        }
        string name = _assembly.GetName().FullName;
        context.Unload();
        s_loadedAssembly.Remove(name);
    }

    public static bool TryGetLoaded(string name, out Assembly? assembly) =>
        s_loadedAssembly.TryGetValue(name, out assembly);
}
