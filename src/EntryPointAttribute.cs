namespace Loader;

public enum ModulePriority
{
    Library,
    Front,
    High,
    Normal
}

public abstract class ModuleEntryPointAttributeBase
    : Attribute,
        IComparer<ModuleEntryPointAttributeBase>
{
    internal abstract IModule CreateInstance();
    internal ModulePriority Priority { get; set; }

    //for sort
    public int Compare(ModuleEntryPointAttributeBase? x, ModuleEntryPointAttributeBase? y)
    {
        if (ReferenceEquals(x, y))
            return 0;
        if (ReferenceEquals(null, y))
            return 1;
        if (ReferenceEquals(null, x))
            return -1;
        return x.Priority.CompareTo(y.Priority);
    }
}

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class ModuleEntryPointAttribute<T> : ModuleEntryPointAttributeBase
    where T : IModule, new()
{
    public ModuleEntryPointAttribute(ModulePriority priority = ModulePriority.Normal)
    {
        Priority = priority;
    }

    internal override IModule CreateInstance() => new T();
}
