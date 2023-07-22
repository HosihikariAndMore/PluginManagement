namespace Loader;

public interface IPlugin : IDisposable
{
    void Initialize();
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    void IDisposable.Dispose() { }
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
}
