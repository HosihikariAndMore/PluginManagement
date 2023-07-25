using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.PluginManager;

public sealed class AssemblyPlugin : Plugin
{
    internal const string PluginDirectoryPath = "plugins";
    internal const string LibraryDirectoryPath = "lib";

    internal static readonly List<AssemblyPlugin> s_plugins;
    private static readonly Dictionary<string, Assembly> s_loadedAssembly;
    private Assembly? _assembly;

    public event EventHandler? Unloading;

    static AssemblyPlugin()
    {
        s_plugins = new();
        s_loadedAssembly = new();

        DirectoryInfo directoryInfo = new(LibraryDirectoryPath);
        foreach (FileInfo file in directoryInfo.EnumerateFiles())
        {
            Assembly assembly;
            try
            {
                assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(
                    file.FullName
                );
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
        PluginLoadContext context = new(_fileInfo.Name, true);
        try
        {
            _assembly = context.LoadFromAssemblyPath(_fileInfo.FullName);
        }
        catch (BadImageFormatException)
        {
            return false;
        }
        AssemblyName name = _assembly.GetName();
        if (string.IsNullOrWhiteSpace(name.Name) || name.Version is null)
        {
            Unload();
            return false;
        }
        Name = name.Name;
        Version = name.Version;
        s_loadedAssembly[name.FullName] = _assembly;
        s_plugins.Add(this);
        return true;
    }

    protected internal override bool Initialize()
    {
        if (_assembly is null)
        {
            throw new NullReferenceException();
        }
        EntryPointAttributeBase? attribute =
            _assembly.GetCustomAttribute<EntryPointAttributeBase>();
        if (attribute is null)
        {
            Unload();
            Console.Error.WriteLine(
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff} ERROR] {Name} initialize failed. (Entry point not found)"
            );
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
            throw new NullReferenceException();
        }

        Unloading?.Invoke(this, EventArgs.Empty);
        AssemblyLoadContext context =
            AssemblyLoadContext.GetLoadContext(_assembly) ??
            throw new NullReferenceException();
        string name = _assembly.GetName().FullName;
        context.Unload();
        s_loadedAssembly.Remove(name);
        s_plugins.Remove(this);
    }

    internal static bool TryGetLoaded(string name, out Assembly? assembly) =>
        s_loadedAssembly.TryGetValue(name, out assembly);
}
