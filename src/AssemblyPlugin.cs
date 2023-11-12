using Hosihikari.PluginManager;
using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.PluginManagement;

public sealed class AssemblyPlugin : Plugin
{
    internal const string PluginDirectoryPath = "plugins";
    internal const string LibraryDirectoryPath = "lib";

    internal static readonly List<AssemblyPlugin> s_plugins;
    private Assembly? _assembly;

    public event EventHandler? Unloading;

    static AssemblyPlugin()
    {
        s_plugins = new();
    }

    internal AssemblyPlugin(FileInfo file) : base(file)
    {
    }

    internal AssemblyPlugin(AssemblyHandler handler) : base(handler.FileInfo)
    {
        _assembly = handler.Assembly ?? throw new NullReferenceException();
    }

    protected internal override void Load()
    {
        if (_assembly is null)
        {
            PluginLoadContext context = new(_fileInfo.Name);
            _assembly = context.LoadFromAssemblyPath(_fileInfo.FullName);
        }
        AssemblyName name = _assembly.GetName();
        if (string.IsNullOrWhiteSpace(name.Name) || name.Version is null)
        {
            Unload();
            throw new BadImageFormatException();
        }
        Name = name.Name;
        Version = name.Version;
        s_plugins.Add(this);
    }

    protected internal override void Initialize()
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
            throw new EntryPointNotFoundException();
        }
        IEntryPoint entry = attribute.CreateInstance();
        entry.Initialize(this);
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
        context.Unload();
        s_plugins.Remove(this);
    }
}
