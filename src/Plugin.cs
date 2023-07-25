namespace Hosihikari.PluginManager;

public abstract class Plugin
{
    public string Name { get; protected set; }
    public Version Version { get; protected set; }

    protected FileInfo _fileInfo;

    protected Plugin(FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
        Name = fileInfo.Name;
        Version = new();
    }

    protected internal abstract bool Load();
    protected internal abstract bool Initialize();
    protected internal abstract void Unload();
}
