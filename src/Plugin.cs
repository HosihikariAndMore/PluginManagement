namespace Hosihikari.PluginManagement;

public abstract class Plugin(FileInfo fileInfo)
{
    protected readonly FileInfo _fileInfo = fileInfo;
    public string Name { get; protected set; } = fileInfo.Name;
    public Version Version { get; protected set; } = new();
    protected internal abstract void Load();
    protected internal abstract void Initialize();
    protected internal abstract void Unload();
}