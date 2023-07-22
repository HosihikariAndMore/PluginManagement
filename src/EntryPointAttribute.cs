namespace Loader;

public abstract class EntryPointAttributeBase : Attribute
{
    internal abstract IPlugin CreateInstance();
}

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class EntryPointAttribute<T> : EntryPointAttributeBase
    where T : IPlugin, new()
{
    internal override IPlugin CreateInstance() => new T();
}

public interface IPlugin
{
    void Initialize();
    bool Unload();
}
