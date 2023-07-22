namespace Loader;

public interface IModule : IDisposable
{
    string Name { get; }
    void Initialize();
    void Unload();
}
