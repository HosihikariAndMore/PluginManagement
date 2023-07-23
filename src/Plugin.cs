namespace Hosihikari.Loader;

public class Plugin
{
    protected internal FileInfo FileInfo { get; }

    protected internal Plugin(FileInfo file)
    {
        FileInfo = file;
    }

    protected internal virtual bool Load()
    {
        throw new NotImplementedException();
    }

    protected internal virtual bool Initialize()
    {
        throw new NotImplementedException();
    }

    protected internal virtual void Unload()
    {
        throw new NotImplementedException();
    }
}
