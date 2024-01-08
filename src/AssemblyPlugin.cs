using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.PluginManagement;

public sealed class AssemblyPlugin : Plugin
{
    internal const string PluginDirectoryPath = "plugins";
    internal static readonly List<AssemblyPlugin> Plugins;
    private Assembly? _assembly;

    static AssemblyPlugin()
    {
        Plugins = [];
    }

    internal AssemblyPlugin(FileInfo file) : base(file)
    {
    }

    public event EventHandler? Unloading;

    protected internal override void Load()
    {
        if (_assembly is not null || Plugins.Contains(this))
        {
            throw new InvalidOperationException();
        }

        PluginLoadContext context = new(_fileInfo.Name);
        _assembly = context.LoadFromAssemblyPath(_fileInfo.FullName);
        AssemblyName name = _assembly.GetName();
        if (string.IsNullOrWhiteSpace(name.Name) || name.Version is null)
        {
            Unload();
            throw new BadImageFormatException();
        }

        Name = name.Name;
        Version = name.Version;
        Plugins.Add(this);
    }

    protected internal override void Initialize()
    {
        if (_assembly is null)
        {
            throw new NullReferenceException();
        }

        EntryPointAttributeBase? attribute = _assembly.GetCustomAttribute<EntryPointAttributeBase>();
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
        if (_assembly is null || !Plugins.Contains(this))
        {
            throw new NullReferenceException();
        }

        Unloading?.Invoke(this, EventArgs.Empty);
        AssemblyLoadContext context =
            AssemblyLoadContext.GetLoadContext(_assembly) ?? throw new NullReferenceException();
        context.Unload();
        Plugins.Remove(this);
    }
}