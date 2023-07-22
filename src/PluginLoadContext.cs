using System.Reflection;
using System.Runtime.Loader;

namespace Hosihikari.Loader;

internal class PluginLoadContext : AssemblyLoadContext
{
    public static AssemblyLoadContext LoaderContext { get; private set; }
    public static Assembly LoaderAssembly { get; private set; }

    static PluginLoadContext()
    {
        LoaderAssembly = Assembly.GetExecutingAssembly();
        LoaderContext = GetLoadContext(LoaderAssembly) ?? throw new NullReferenceException();
    }

    //AssemblyDependencyResolver resolver;
    //FileInfo fileInfo;

    public PluginLoadContext(string? name, /*FileInfo file,*/ bool isCollectible = false)
        : base(name, isCollectible)
    {
        //fileInfo = file;
        //resolver = new(fileInfo.FullName);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (assemblyName.Name == LoaderAssembly.GetName().Name)
            return LoaderAssembly;

        return base.Load(assemblyName);
    }


    //todo

    /*protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
        return base.LoadUnmanagedDll(unmanagedDllName);
    }*/
}
