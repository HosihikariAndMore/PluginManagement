namespace Hosihikari.PluginManager;

public abstract class Plugin
{
    public string Name { get; protected internal set; }
    public Version Version { get; protected internal set; }

    protected internal FileInfo _fileInfo;

    protected internal Plugin(FileInfo fileInfo)
    {
        _fileInfo = fileInfo;
        Name = string.Empty;
        Version = new();
    }

    protected internal abstract bool Load();
    protected internal abstract bool Initialize();
    protected internal abstract void Unload();
}
