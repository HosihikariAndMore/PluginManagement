namespace Hosihikari.Loader;

public abstract class EntryPointAttributeBase : Attribute
{
    internal abstract IPlugin CreateInstance();
}

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class EntryPointAttribute<T> : EntryPointAttributeBase where T
    : IPlugin, new()
{
    internal override IPlugin CreateInstance() => new T();
}
