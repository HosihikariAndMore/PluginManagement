namespace Hosihikari.PluginManagement;

public abstract class EntryPointAttributeBase : Attribute
{
    public abstract IEntryPoint CreateInstance();
}

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class EntryPointAttribute<T> : EntryPointAttributeBase where T : IEntryPoint, new()
{
    public override IEntryPoint CreateInstance()
    {
        return new T();
    }
}

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class EntryPointAttribute(Type pluginType) : EntryPointAttributeBase
{
    public override IEntryPoint CreateInstance()
    {
        return (Activator.CreateInstance(pluginType) as IEntryPoint) ?? throw new EntryPointNotFoundException("Entry point not found.");
    }
}