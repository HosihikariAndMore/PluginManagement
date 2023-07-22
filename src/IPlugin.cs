namespace Loader;

public interface IPlugin : IDisposable
{
    void Initialize();
    void Unload();
}
