using System.Reflection;
using System.Runtime.Loader;

namespace Loader;

internal static class PluginManager
{
    public static List<AssemblyLoadContext> PluginContexts { get; }

    static PluginManager()
    {
        PluginContexts = new();
    }

    public static bool LoadPlugin(string path)
    {
        AssemblyLoadContext loadContext = new(path);
        Assembly assembly = loadContext.LoadFromAssemblyPath(path);
        Type[] types = assembly.GetTypes();
        foreach (Type type in types)
        {
            MethodInfo[] methods = type.GetMethods();
            foreach (MethodInfo method in methods)
            {
                if (method.GetCustomAttribute<EntryPointAttribute>() is null || method.ReturnType is not null || method.GetParameters().LongLength > 0)
                {
                    continue;
                }
                method.Invoke(null, null);
                PluginContexts.Add(loadContext);
                return true;
            }
        }
        return false;
    }
}
