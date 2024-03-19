using System.Reflection;

namespace Hosihikari.PluginManagement;

public sealed class AssemblyPlugin : Plugin
{
    internal const string PluginDirectoryPath = "plugins";
    internal static readonly List<AssemblyPlugin> Plugins;
    private Assembly? _assembly;
    private EntryPointAttributeBase? _attribute;
    private PluginLoadContext? _context;

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
        _context = new(_fileInfo);
        _assembly = _context.LoadFromAssemblyPath(_fileInfo.FullName);
        Plugins.Add(this);
        _attribute = _assembly.GetCustomAttribute<EntryPointAttributeBase>();
        if (_attribute is null)
        {
            Unload();
            throw new EntryPointNotFoundException();
        }

        AssemblyName name = _assembly.GetName();
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

        IEntryPoint entry = _attribute!.CreateInstance();
        entry.Initialize(this);
    }

    protected internal override void Unload()
    {
        if (!Plugins.Contains(this))
        {
            throw new NullReferenceException();
        }

        Unloading?.Invoke(this, EventArgs.Empty);
        _context!.Unload();
        Plugins.Remove(this);
    }
}