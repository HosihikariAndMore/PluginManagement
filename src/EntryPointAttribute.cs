namespace Hosihikari.PluginManagement;

public abstract class EntryPointAttributeBase : Attribute
{
    internal abstract IEntryPoint CreateInstance();
}

[AttributeUsage(AttributeTargets.Assembly)]
public class EntryPointAttribute<T> : EntryPointAttributeBase where T : IEntryPoint, new()
{
    internal override IEntryPoint CreateInstance()
    {
        return new T();
    }
}