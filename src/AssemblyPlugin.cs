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
            return;
        }

        PluginLoadContext context = new(_fileInfo);
        _assembly = context.LoadFromAssemblyPath(_fileInfo.FullName);
        AssemblyName name = _assembly.GetName();
        Plugins.Add(this);
        if (string.IsNullOrWhiteSpace(name.Name) || name.Version is null)
        {
            Unload();
            throw new BadImageFormatException();
        }

        Name = name.Name;
        Version = name.Version;
    }

    protected internal override void Initialize()
    {
        if (!Plugins.Contains(this))
        {
            throw new NullReferenceException();
        }

        EntryPointAttributeBase? attribute = _assembly!.GetCustomAttribute<EntryPointAttributeBase>();
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
        if (!Plugins.Contains(this))
        {
            throw new NullReferenceException();
        }

        AssemblyLoadContext context =
            AssemblyLoadContext.GetLoadContext(_assembly!) ?? throw new NullReferenceException();
        Unloading?.Invoke(this, EventArgs.Empty);
        context.Unload();
        Plugins.Remove(this);
    }
}