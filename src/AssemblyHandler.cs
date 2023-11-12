using Hosihikari.PluginManagement;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Hosihikari.PluginManager;

internal class AssemblyHandler
{
    public FileInfo FileInfo { get; private set; }

    public Assembly? Assembly { get; private set; }

    public bool IsPluginAssembly { get; private set; }

    public AssemblyHandler(FileInfo file)
    {
        FileInfo = file;
    }

    [MemberNotNullWhen(true, nameof(Assembly))]
    public bool TryLoad([NotNullWhen(false)] out Exception? exception)
    {
        PluginLoadContext? context = null;
        try
        {
            context = new(FileInfo.Name);
            Assembly = context.LoadFromAssemblyPath(FileInfo.FullName);
        }
        catch (Exception ex)
        {
            context?.Unload();
            exception = ex;
            return false;
        }

        if (Assembly.GetCustomAttribute<EntryPointAttributeBase>() is not null)
            IsPluginAssembly = true;
        exception = null;
        return true;
    }

    public AssemblyPlugin CreatePlugin()
    {
        if (IsPluginAssembly is false)
            throw new InvalidOperationException();

        return new(this);
    }
}
